namespace CaroGame.DAL.Models
{
    // Một dòng kết quả của câu truy vấn xếp hạng
    public class UserRanking
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public int Score { get; set; }
        public int OnlineWins { get; set; }
    }
}
