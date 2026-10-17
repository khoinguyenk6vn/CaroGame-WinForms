using System.ComponentModel;

namespace CaroGame.BLL.Dtos
{
    // Một dòng trong bảng lịch sử đấu. DisplayName là tên cột hiện trên DataGridView.
    public class HistoryItemDto
    {
        [DisplayName("Thời gian")]
        public string PlayedAt { get; set; } = "";

        [DisplayName("Chế độ")]
        public string Mode { get; set; } = "";

        [DisplayName("Đối thủ")]
        public string Opponent { get; set; } = "";

        [DisplayName("Kết quả")]
        public string Result { get; set; } = "";

        [DisplayName("Lý do")]
        public string Reason { get; set; } = "";

        [DisplayName("Số nước")]
        public int MoveCount { get; set; }

        // Chỉ trận Online có điểm, PvE và Offline để trống
        [DisplayName("Điểm")]
        public string ScoreChange { get; set; } = "";
    }
}
