namespace CaroGame.BLL.Services
{
    // Một người đang đăng nhập
    public class OnlineUser
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string ConnectionId { get; set; } = "";
        public string Token { get; set; } = "";
    }
}
