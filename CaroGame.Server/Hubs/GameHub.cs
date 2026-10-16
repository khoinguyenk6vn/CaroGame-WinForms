using CaroGame.Server.Data;
using CaroGame.Server.Services;
using CaroGame.Shared.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace CaroGame.Server.Hubs
{
    // Hub SignalR: client gọi các hàm public ở đây bằng tên hàm, ví dụ InvokeAsync("Login", ...)
    public class GameHub : Hub
    {
        private readonly AccountService accountService;

        public GameHub(AccountService accountService)
        {
            this.accountService = accountService;
        }

        // ===================== Tài khoản =====================

        public async Task<AccountResult> Register(string username, string email, string password)
        {
            return await accountService.RegisterAsync(username, email, password);
        }

        public async Task<AccountResult> VerifyEmail(string email, string code)
        {
            return await accountService.VerifyEmailAsync(email, code);
        }

        public async Task<AccountResult> ResendCode(string email, CodePurpose purpose)
        {
            return await accountService.ResendCodeAsync(email, purpose);
        }

        public async Task<AccountResult> ForgotPassword(string email)
        {
            return await accountService.ForgotPasswordAsync(email);
        }

        public async Task<AccountResult> CheckResetCode(string email, string code)
        {
            return await accountService.CheckResetCodeAsync(email, code);
        }

        public async Task<AccountResult> ResetPassword(string email, string code, string newPassword)
        {
            return await accountService.ResetPasswordAsync(email, code, newPassword);
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            User? user = await accountService.CheckLoginAsync(username, password);
            if (user == null)
            {
                return new LoginResult { Message = "Sai username hoặc mật khẩu." };
            }

            // Chưa xác minh email: gửi mã mới rồi bắt nhập mã, chưa cho vào game.
            // Nếu vừa gửi chưa đủ 60 giây thì không gửi lại, người chơi dùng mã đã nhận.
            if (!user.IsEmailVerified)
            {
                await accountService.SendCodeAsync(user, CodePurpose.Register);
                return new LoginResult
                {
                    NeedEmailVerification = true,
                    Email = user.Email,
                    Message = "Tài khoản chưa xác minh email."
                };
            }

            string? token = SessionStore.TryAdd(user.Id, user.Username, Context.ConnectionId);
            if (token == null)
            {
                return new LoginResult { Message = "Tài khoản đang đăng nhập ở nơi khác." };
            }

            return new LoginResult
            {
                Success = true,
                Token = token,
                Username = user.Username,
                Score = user.Score
            };
        }

        // ===================== Kết nối =====================

        // SignalR tự gọi hàm này khi client ngắt kết nối (tắt app, mất mạng...)
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            SessionStore.RemoveByConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
