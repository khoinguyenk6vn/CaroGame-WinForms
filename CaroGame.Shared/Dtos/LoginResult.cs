namespace CaroGame.Shared.Dtos
{
    // Kết quả đăng nhập
    public class LoginResult : AccountResult
    {
        // true nếu tài khoản chưa xác minh email: client chuyển sang form nhập mã
        public bool NeedEmailVerification { get; set; }
        public string Email { get; set; } = "";

        // Các thông tin dưới đây chỉ có khi đăng nhập thành công
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public int Score { get; set; }
    }
}
