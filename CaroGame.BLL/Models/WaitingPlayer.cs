namespace CaroGame.BLL.Models
{
    // Một người đang trong hàng chờ ghép trận
    public class WaitingPlayer
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public int Score { get; set; }
        public string ConnectionId { get; set; } = "";
        public int SecondsWaited { get; set; }
    }
}
