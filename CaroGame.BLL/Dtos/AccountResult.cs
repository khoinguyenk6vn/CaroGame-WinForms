namespace CaroGame.BLL.Dtos
{
    // Kết quả server trả về cho các thao tác tài khoản (đăng ký, xác minh mã, đổi mật khẩu...)
    public class AccountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
    }
}
