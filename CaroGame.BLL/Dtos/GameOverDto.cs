using CaroGame.BLL.Models;

namespace CaroGame.BLL.Dtos
{
    // Kết quả trận Online gửi cho từng người
    public class GameOverDto
    {
        public MatchResult Result { get; set; }
        public EndReason EndReason { get; set; }

        // Điểm thực tế thay đổi (đang 1000 mà thua thì là 0)
        public int ScoreChange { get; set; }
        public int NewScore { get; set; }
    }
}
