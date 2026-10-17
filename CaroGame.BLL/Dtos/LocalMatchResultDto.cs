using CaroGame.BLL.Game;

namespace CaroGame.BLL.Dtos
{
    // Kết quả trận PvE/Offline client gửi lên server để lưu lịch sử (chỉ khi đã đăng nhập)
    public class LocalMatchResultDto
    {
        public GameMode Mode { get; set; }

        // Kết quả của người chơi 1 (người đang đăng nhập)
        public MatchResult Result { get; set; }

        public EndReason EndReason { get; set; }
        public int MoveCount { get; set; }
    }
}
