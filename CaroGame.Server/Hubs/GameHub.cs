using CaroGame.BLL.Dtos;
using CaroGame.BLL.Online;
using CaroGame.BLL.Services;
using Microsoft.AspNetCore.SignalR;

namespace CaroGame.Server.Hubs
{
    // Hub SignalR: client gọi các hàm public ở đây bằng tên hàm, ví dụ InvokeAsync("Login", ...).
    // Hub chỉ nhận yêu cầu rồi chuyển cho tầng BLL xử lý, không chứa nghiệp vụ.
    // Context.ConnectionId: mã kết nối của máy đang gọi, SignalR tự cấp; BLL dùng nó để biết ai đang gọi.
    public class GameHub : Hub
    {
        private readonly AccountService accountService;
        private readonly PlayerService playerService;
        private readonly OnlineGameManager gameManager;

        public GameHub(AccountService accountService, PlayerService playerService, OnlineGameManager gameManager)
        {
            this.accountService = accountService;
            this.playerService = playerService;
            this.gameManager = gameManager;
        }

        // ===================== Tài khoản =====================

        public async Task<AccountResult> Register(string username, string email, string password, string confirmPassword)
        {
            return await accountService.RegisterAsync(username, email, password, confirmPassword);
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

        public async Task<AccountResult> ResetPassword(string email, string code, string newPassword, string confirmPassword)
        {
            return await accountService.ResetPasswordAsync(email, code, newPassword, confirmPassword);
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            return await accountService.LoginAsync(username, password, Context.ConnectionId);
        }

        public void Logout()
        {
            accountService.Logout(Context.ConnectionId);
        }

        public async Task<bool> Reconnect(string token)
        {
            return await accountService.ReconnectAsync(token, Context.ConnectionId);
        }

        // ===================== Profile, lịch sử, bảng xếp hạng =====================

        public async Task<ProfileDto?> GetProfile()
        {
            return await playerService.GetProfileAsync(Context.ConnectionId);
        }

        public async Task<List<HistoryItemDto>> GetHistory()
        {
            return await playerService.GetHistoryAsync(Context.ConnectionId);
        }

        public async Task<List<LeaderboardRowDto>> GetLeaderboard()
        {
            return await playerService.GetLeaderboardAsync(Context.ConnectionId);
        }

        public async Task SaveLocalMatch(LocalMatchResultDto result)
        {
            await playerService.SaveLocalMatchAsync(Context.ConnectionId, result);
        }

        // ===================== PvP Online =====================

        public async Task<string> JoinQueue()
        {
            return await gameManager.JoinQueueAsync(Context.ConnectionId);
        }

        public void LeaveQueue()
        {
            gameManager.LeaveQueue(Context.ConnectionId);
        }

        public async Task VoteRule(bool agree)
        {
            await gameManager.VoteRuleAsync(Context.ConnectionId, agree);
        }

        public async Task PlayMove(int row, int col)
        {
            await gameManager.PlayMoveAsync(Context.ConnectionId, row, col);
        }

        public async Task<string> RequestUndo()
        {
            return await gameManager.RequestUndoAsync(Context.ConnectionId);
        }

        public async Task AnswerUndo(bool accept)
        {
            await gameManager.AnswerUndoAsync(Context.ConnectionId, accept);
        }

        public async Task SendChat(string text)
        {
            await gameManager.SendChatAsync(Context.ConnectionId, text);
        }

        public async Task Surrender()
        {
            await gameManager.SurrenderAsync(Context.ConnectionId);
        }

        // ===================== Kết nối =====================

        // SignalR tự gọi hàm này khi client ngắt kết nối (tắt app, mất mạng...)
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await accountService.DisconnectAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
