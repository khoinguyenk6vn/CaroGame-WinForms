using CaroGame.BLL.Game;

namespace CaroGame.BLL.Online
{
    // Một người chơi trong trận Online
    public class OnlinePlayer
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string ConnectionId { get; set; } = "";
        public Stone Stone { get; set; }

        // Trả lời câu hỏi luật chặn 2 đầu: null là chưa trả lời
        public bool? AgreesBlockRule { get; set; }

        // Số lần bị từ chối xin Undo trong trận này
        public int UndoRejections { get; set; }

        public bool IsDisconnected { get; set; }
    }
}
