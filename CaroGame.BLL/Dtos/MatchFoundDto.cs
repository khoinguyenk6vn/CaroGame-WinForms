using CaroGame.BLL.Game;

namespace CaroGame.BLL.Dtos
{
    // Server gửi cho 2 người khi ghép được trận Online
    public class MatchFoundDto
    {
        public string XName { get; set; } = "";
        public string OName { get; set; } = "";

        // Quân của người nhận tin này
        public Stone MyStone { get; set; }
    }
}
