using System.ComponentModel;

namespace CaroGame.BLL.Dtos
{
    // Một dòng trong bảng xếp hạng. DisplayName là tên cột hiện trên DataGridView.
    public class LeaderboardRowDto
    {
        [DisplayName("Hạng")]
        public int Position { get; set; }

        [DisplayName("Tên")]
        public string Username { get; set; } = "";

        [DisplayName("Điểm")]
        public int Score { get; set; }

        [DisplayName("Bậc")]
        public string RankName { get; set; } = "";
    }
}
