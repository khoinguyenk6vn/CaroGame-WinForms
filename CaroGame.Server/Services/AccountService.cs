using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CaroGame.Server.Data;
using CaroGame.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.Server.Services
{
    // Xử lý tài khoản: đăng ký, kiểm tra đăng nhập, mã xác minh email, đổi mật khẩu.
    // Dùng thẳng AppDbContext để đọc/ghi CSDL.
    public class AccountService
    {
        private const int StartingScore = 1000;
        private const int DefaultAvatarCount = 20;
        private const int CodeLifetimeMinutes = 5;
        private const int ResendWaitSeconds = 60;
        private const int MaxFailedAttempts = 5;

        private readonly AppDbContext db;
        private readonly EmailService emailService;

        public AccountService(AppDbContext db, EmailService emailService)
        {
            this.db = db;
            this.emailService = emailService;
        }

        // ===================== Đăng ký =====================

        public async Task<AccountResult> RegisterAsync(string username, string email, string password)
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

            if (password.Length < 6 || password.Length > 50)
            {
                return Fail("Mật khẩu phải dài 6-50 ký tự.");
            }

            // SQL Server mặc định so sánh chuỗi không phân biệt hoa thường,
            // nên "Abc" và "abc" bị tính là trùng
            if (await db.Users.AnyAsync(u => u.Username == username))
            {
                return Fail("Username đã tồn tại.");
            }

            if (await db.Users.AnyAsync(u => u.Email == email))
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
            db.Users.Add(user);
            await db.SaveChangesAsync();

            return await SendCodeAsync(user, CodePurpose.Register);
        }

        // ===================== Đăng nhập =====================

        // Trả về user nếu đúng username và mật khẩu, ngược lại trả về null
        public async Task<User?> CheckLoginAsync(string username, string password)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null;
            }

            // So mật khẩu nhập vào với mật khẩu đã hash trong CSDL
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        // ===================== Mã xác minh =====================

        public async Task<AccountResult> ResendCodeAsync(string email, CodePurpose purpose)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            return await SendCodeAsync(user, purpose);
        }

        // Tạo mã 6 số mới, gửi qua email, rồi lưu vào CSDL (mã cũ cùng mục đích bị xóa)
        public async Task<AccountResult> SendCodeAsync(User user, CodePurpose purpose)
        {
            VerificationCode? oldCode = await FindCodeAsync(user, purpose);
            if (oldCode != null)
            {
                int secondsSinceSent = (int)(DateTime.Now - oldCode.CreatedAt).TotalSeconds;
                if (secondsSinceSent < ResendWaitSeconds)
                {
                    return Fail("Vui lòng chờ " + (ResendWaitSeconds - secondsSinceSent) + " giây rồi gửi lại mã.");
                }

                db.VerificationCodes.Remove(oldCode);
            }

            // Sinh số ngẫu nhiên an toàn từ 000000 đến 999999
            string code = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");

            try
            {
                await emailService.SendCodeAsync(user.Email, code);
            }
            catch (Exception)
            {
                return Fail("Không gửi được email, hãy thử lại sau.");
            }

            db.VerificationCodes.Add(new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                Purpose = purpose,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(CodeLifetimeMinutes),
                FailedAttempts = 0
            });
            await db.SaveChangesAsync();

            return Ok("Mã xác minh đã được gửi tới " + user.Email);
        }

        // Xác minh email sau khi đăng ký
        public async Task<AccountResult> VerifyEmailAsync(string email, string code)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
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
            await db.SaveChangesAsync();
            return Ok("Xác minh email thành công.");
        }

        // Kiểm tra mã đúng hay sai. Nếu sai thì tăng số lần sai, sai 5 lần thì hủy mã.
        // deleteWhenCorrect: mã đúng thì xóa luôn để không dùng lại được.
        private async Task<AccountResult> CheckCodeAsync(User user, CodePurpose purpose, string code, bool deleteWhenCorrect)
        {
            VerificationCode? savedCode = await FindCodeAsync(user, purpose);
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
                    db.VerificationCodes.Remove(savedCode);
                    await db.SaveChangesAsync();
                    return Fail("Nhập sai 5 lần, mã đã bị hủy. Hãy bấm gửi lại mã.");
                }

                await db.SaveChangesAsync();
                return Fail("Mã không đúng. Còn " + (MaxFailedAttempts - savedCode.FailedAttempts) + " lần thử.");
            }

            if (deleteWhenCorrect)
            {
                db.VerificationCodes.Remove(savedCode);
                await db.SaveChangesAsync();
            }

            return Ok("Mã đúng.");
        }

        private async Task<VerificationCode?> FindCodeAsync(User user, CodePurpose purpose)
        {
            return await db.VerificationCodes.FirstOrDefaultAsync(c => c.UserId == user.Id && c.Purpose == purpose);
        }

        // ===================== Quên mật khẩu =====================

        public async Task<AccountResult> ForgotPasswordAsync(string email)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            return await SendCodeAsync(user, CodePurpose.ResetPassword);
        }

        // Kiểm tra mã trước khi cho mở form đặt mật khẩu mới. Mã đúng vẫn giữ lại để dùng ở bước đổi mật khẩu.
        public async Task<AccountResult> CheckResetCodeAsync(string email, string code)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Fail("Email không tồn tại.");
            }

            return await CheckCodeAsync(user, CodePurpose.ResetPassword, code, false);
        }

        // Kiểm tra mã một lần nữa (để không ai đổi được mật khẩu mà không có mã), rồi đổi mật khẩu
        public async Task<AccountResult> ResetPasswordAsync(string email, string code, string newPassword)
        {
            if (newPassword.Length < 6 || newPassword.Length > 50)
            {
                return Fail("Mật khẩu phải dài 6-50 ký tự.");
            }

            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
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
            await db.SaveChangesAsync();
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
