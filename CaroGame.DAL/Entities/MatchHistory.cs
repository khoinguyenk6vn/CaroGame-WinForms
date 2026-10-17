using System.ComponentModel.DataAnnotations;

namespace CaroGame.DAL.Entities
{
    // Bảng MatchHistories: mỗi dòng là kết quả 1 trận của 1 người chơi.
    // Trận Online lưu 2 dòng (mỗi người 1 dòng), trận PvE/Offline lưu 1 dòng.
    public class MatchHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        // Đối thủ (chỉ có ở trận Online). PvE và Offline để null.
        public int? OpponentId { get; set; }
        public User? Opponent { get; set; }

        // "PvE", "PvPOffline", "PvPOnline"
        [MaxLength(20)]
        public string Mode { get; set; } = "";

        // "Win", "Lose", "Draw"
        [MaxLength(10)]
        public string Result { get; set; } = "";

        // "FiveInRow", "Timeout", "Forfeit", "Disconnect", "Draw"
        [MaxLength(20)]
        public string EndReason { get; set; } = "";

        public bool BlockBothEndsRule { get; set; }

        public int MoveCount { get; set; }

        // Điểm thay đổi, chỉ có ở trận Online
        public int? ScoreChange { get; set; }

        public DateTime PlayedAt { get; set; }
    }
}
