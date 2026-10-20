using CaroGame.BLL.Models;

namespace CaroGame.BLL.Dtos
{
    // Server gửi cho 2 người khi ghép được trận Online
    public class MatchFoundDto
    {
        public string XName { get; set; } = "";
        public string OName { get; set; } = "";

        // Quân của người nhận tin này
        public Stone MyStone { get; set; }

        // Điểm của 2 người (để hiện logo rank)
        public int XScore { get; set; }
        public int OScore { get; set; }

        // Avatar của 2 người: số thứ tự ảnh mặc định, và đường dẫn ảnh đã upload (rỗng nếu chưa upload)
        public int XDefaultAvatarIndex { get; set; }
        public string XAvatarPath { get; set; } = "";
        public int ODefaultAvatarIndex { get; set; }
        public string OAvatarPath { get; set; } = "";
    }
}
