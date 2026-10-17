using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CaroGame.BLL.Dtos;
using CaroGame.BLL.Online;
using CaroGame.DAL.Entities;
using CaroGame.DAL.DataAccess;
using CaroGame.BLL.Models;

namespace CaroGame.BLL.Services
{
    // Nghiệp vụ tài khoản: đăng ký, đăng nhập, mã xác minh email, đổi mật khẩu.
    // Không truy vấn CSDL trực tiếp, mọi thao tác dữ liệu đều gọi qua UserDAL và VerificationCodeDAL.
    public class AccountService
    {
        private const int StartingScore = 1000;
        private const int DefaultAvatarCount = 20;
        private const int CodeLifetimeMinutes = 5;
        private const int ResendWaitSeconds = 60;
        private const int MaxFailedAttempts = 5;

        private readonly UserDAL userDal;
        private readonly VerificationCodeDAL codeDal;
        private readonly EmailService emailService;
        private readonly OnlineGameManager gameManager;

        public AccountService(UserDAL userDal, VerificationCodeDAL codeDal, EmailService emailService, OnlineGameManager gameManager)
        {
            this.userDal = userDal;
            this.codeDal = codeDal;
            this.emailService = emailService;
            this.gameManager = gameManager;
        }

        // ===================== Đăng ký =====================

        public async Task<AccountResult> RegisterAsync(string username, string email, string password, string confirmPassword)
        {
            // Username: 4-20 ký tự, chỉ gồm chữ không dấu, số và dấu _
            if (!Regex.IsMatch(username, "^[A-Za-z0-9_]{4,20}$"))
            {
                return Fail("Username phải dài 4-20 ký tự, chỉ gồm chữ không dấu, số và dấu _.");
            }

            if (!MailAddress.TryCreate(email, out _))
            {
                return Fail("Email không hợp lệ.");
            }

            AccountResult passwordCheck = CheckNewPassword(password, confirmPassword);
            if (!passwordCheck.Success)
            {
                return passwordCheck;
            }

            if (await userDal.UsernameExistsAsync(username))
            {
                return Fail("Username đã tồn tại.");
            }

            if (await userDal.EmailExistsAsync(email))
            {
                return Fail("Email đã được dùng cho tài khoản khác.");
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsEmailVerified = false,
                Score = StartingScore,
                DefaultAvatarIndex = Random.Shared.Next(DefaultAvatarCount),
                CreatedAt = DateTime.Now
            };
            await userDal.AddAsync(user);

            // Không gửi được mã xác minh thì xóa tài khoản vừa tạo,
            // để người chơi đăng ký lại được với cùng username và email
            AccountResult sendResult = await SendCodeAsync(user, CodePurpose.Register);
            if (!sendResult.Success)
            {
                await userDal.DeleteAsync(user);
            }
            return sendResult;
        }

        // Mật khẩu 6-50 ký tự và phải nhập lại giống nhau
        private AccountResult CheckNewPassword(string password, string confirmPassword)
        {
            if (password.Length < 6 || password.Length > 50)
            {
                return Fail("Mật khẩu phải dài 6-50 ký tự.");
            }

            if (password != confirmPassword)
            {
                return Fail("Mật khẩu nhập lại không khớp.");
            }

            return Ok("");
        }

        // ===================== Đăng nhập =====================

        // connectionId: mã kết nối SignalR của máy đang đăng nhập, dùng để chặn đăng nhập 2 máy
        public async Task<LoginResult> LoginAsync(string username, string password, string connectionId)
        {
            User? user = await userDal.GetByUsernameAsync(username);

            // So mật khẩu nhập vào với mật khẩu đã hash trong CSDL
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return new LoginResult { Message = "Sai username hoặc mật khẩu." };
            }

            // Chưa xác minh email: gửi mã mới rồi bắt nhập mã, chưa cho vào game.
            // Nếu vừa gửi chưa đủ 60 giây thì không gửi lại, người chơi dùng mã đã nhận.
            if (!user.IsEmailVerified)
            {
                AccountResult sendResult = await SendCodeAsync(user, CodePurpose.Register);

                // Gửi mã không được và cũng không còn mã cũ dùng được thì báo lỗi, không mở form nhập mã
                if (!sendResult.Success && !await HasValidCodeAsync(user, CodePurpose.Register))
                {
                    return new LoginResult { Message = sendResult.Message };
                }

                return new LoginResult
                {
                    NeedEmailVerification = true,
                    Email = user.Email,
                    Message = "Tài khoản chưa xác minh email."
                };
            }

            string? token = SessionStore.TryAdd(user.Id, user.Username, connectionId);
            if (token == null)
            {
                return new LoginResult { Message = "Tài khoản đang đăng nhập ở nơi khác." };
            }

            return new LoginResult
            {
                Success = true,
                Token = token,
                Username = user.Username,
                Score = user.Score,
                DefaultAvatarIndex = user.DefaultAvatarIndex,
                AvatarPath = user.AvatarPath ?? ""
            };
        }

        public void Logout(string connectionId)
        {
            SessionStore.RemoveByConnection(connectionId);
        }

        // Ngoài trận: mất kết nối là giải phóng phiên ngay.
        // Trong trận: giữ phiên để chờ kết nối lại 30 giây, hết 30 giây thì OnlineGameManager xử FF và giải phóng.
        public async Task DisconnectAsync(string connectionId)
        {
            bool waitForReconnect = await gameManager.HandleDisconnectAsync(connectionId);
            if (!waitForReconnect)
            {
                SessionStore.RemoveByConnection(connectionId);
            }
        }

        // SignalR tự kết nối lại thì mã kết nối đổi: client gửi token để gắn lại phiên với mã kết nối mới.
        // Trả về false nếu phiên đã bị giải phóng (phải đăng nhập lại).
        public async Task<bool> ReconnectAsync(string token, string newConnectionId)
        {
            OnlineUser? onlineUser = SessionStore.FindByToken(token);
            if (onlineUser == null)
            {
                return false;
            }

            SessionStore.UpdateConnection(token, newConnectionId);
            await gameManager.HandleReconnectAsync(onlineUser.UserId, newConnectionId);
            return true;
        }

        // ===================== Mã xác minh =====================

        public async Task<AccountResult> ResendCodeAsync(string email, CodePurpose purpose)
        {
            User? user = await userDal.GetByEmailAsync(email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            return await SendCodeAsync(user, purpose);
        }

        // Tạo mã 6 số mới, gửi qua email, rồi lưu vào CSDL (mã cũ cùng mục đích bị xóa)
        private async Task<AccountResult> SendCodeAsync(User user, CodePurpose purpose)
        {
            VerificationCode? oldCode = await codeDal.GetAsync(user.Id, purpose.ToString());
            if (oldCode != null)
            {
                int secondsSinceSent = (int)(DateTime.Now - oldCode.CreatedAt).TotalSeconds;
                if (secondsSinceSent < ResendWaitSeconds)
                {
                    return Fail("Vui lòng chờ " + (ResendWaitSeconds - secondsSinceSent) + " giây rồi gửi lại mã.");
                }
            }

            // Sinh số ngẫu nhiên an toàn từ 000000 đến 999999
            string code = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");

            try
            {
                await emailService.SendCodeAsync(user.Email, code);
            }
            catch (Exception ex)
            {
                // Ghi lý do thật ra cửa sổ server để người quản lý server sửa (thường là sai Gmail hoặc App Password)
                Console.WriteLine("[Loi gui mail] " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "")
                    + " => kiem tra Email:Address va Email:AppPassword trong appsettings.Local.json");
                return Fail("Không gửi được email xác minh: máy chủ chưa cấu hình đúng Gmail, hãy báo người quản lý server rồi thử lại.");
            }

            // Gửi thành công mới xóa mã cũ, để nếu gửi lỗi thì mã cũ (nếu còn hạn) vẫn dùng được
            if (oldCode != null)
            {
                await codeDal.DeleteAsync(oldCode);
            }

            await codeDal.AddAsync(new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                Purpose = purpose.ToString(),
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(CodeLifetimeMinutes),
                FailedAttempts = 0
            });

            return Ok("Mã xác minh đã được gửi tới " + user.Email);
        }

        // Còn mã chưa hết hạn: người chơi vẫn dùng được mã đã nhận trước đó
        private async Task<bool> HasValidCodeAsync(User user, CodePurpose purpose)
        {
            VerificationCode? savedCode = await codeDal.GetAsync(user.Id, purpose.ToString());
            return savedCode != null && DateTime.Now <= savedCode.ExpiresAt;
        }

        // Xác minh email sau khi đăng ký
        public async Task<AccountResult> VerifyEmailAsync(string email, string code)
        {
            User? user = await userDal.GetByEmailAsync(email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            AccountResult result = await CheckCodeAsync(user, CodePurpose.Register, code, true);
            if (!result.Success)
            {
                return result;
            }

            user.IsEmailVerified = true;
            await userDal.UpdateAsync(user);
            return Ok("Xác minh email thành công.");
        }

        // Kiểm tra mã đúng hay sai. Nếu sai thì tăng số lần sai, sai 5 lần thì hủy mã.
        // deleteWhenCorrect: mã đúng thì xóa luôn để không dùng lại được.
        private async Task<AccountResult> CheckCodeAsync(User user, CodePurpose purpose, string code, bool deleteWhenCorrect)
        {
            VerificationCode? savedCode = await codeDal.GetAsync(user.Id, purpose.ToString());
            if (savedCode == null)
            {
                return Fail("Mã không còn hiệu lực, hãy bấm gửi lại mã.");
            }

            if (DateTime.Now > savedCode.ExpiresAt)
            {
                return Fail("Mã đã hết hạn, hãy bấm gửi lại mã.");
            }

            if (savedCode.Code != code)
            {
                savedCode.FailedAttempts++;

                if (savedCode.FailedAttempts >= MaxFailedAttempts)
                {
                    await codeDal.DeleteAsync(savedCode);
                    return Fail("Nhập sai 5 lần, mã đã bị hủy. Hãy bấm gửi lại mã.");
                }

                await codeDal.UpdateAsync(savedCode);
                return Fail("Mã không đúng. Còn " + (MaxFailedAttempts - savedCode.FailedAttempts) + " lần thử.");
            }

            if (deleteWhenCorrect)
            {
                await codeDal.DeleteAsync(savedCode);
            }

            return Ok("Mã đúng.");
        }

        // ===================== Quên mật khẩu =====================

        public async Task<AccountResult> ForgotPasswordAsync(string email)
        {
            User? user = await userDal.GetByEmailAsync(email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            return await SendCodeAsync(user, CodePurpose.ResetPassword);
        }

        // Kiểm tra mã trước khi cho mở form đặt mật khẩu mới. Mã đúng vẫn giữ lại để dùng ở bước đổi mật khẩu.
        public async Task<AccountResult> CheckResetCodeAsync(string email, string code)
        {
            User? user = await userDal.GetByEmailAsync(email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            return await CheckCodeAsync(user, CodePurpose.ResetPassword, code, false);
        }

        // Kiểm tra mã một lần nữa (để không ai đổi được mật khẩu mà không có mã), rồi đổi mật khẩu
        public async Task<AccountResult> ResetPasswordAsync(string email, string code, string newPassword, string confirmPassword)
        {
            AccountResult passwordCheck = CheckNewPassword(newPassword, confirmPassword);
            if (!passwordCheck.Success)
            {
                return passwordCheck;
            }

            User? user = await userDal.GetByEmailAsync(email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            AccountResult result = await CheckCodeAsync(user, CodePurpose.ResetPassword, code, true);
            if (!result.Success)
            {
                return result;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await userDal.UpdateAsync(user);
            return Ok("Đổi mật khẩu thành công.");
        }

        // ===================== Hàm phụ =====================

        private static AccountResult Ok(string message)
        {
            return new AccountResult { Success = true, Message = message };
        }

        private static AccountResult Fail(string message)
        {
            return new AccountResult { Success = false, Message = message };
        }
    }
}
