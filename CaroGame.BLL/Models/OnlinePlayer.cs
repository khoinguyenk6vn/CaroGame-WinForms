
namespace CaroGame.BLL.Models
{
    // Một người chơi trong trận Online
    public class OnlinePlayer
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string ConnectionId { get; set; } = "";
        public Stone Stone { get; set; }

        // Câu hỏi luật chặn 2 đầu: đã trả lời chưa, và có đồng ý không
        public bool HasVotedRule { get; set; }
        public bool AgreesBlockRule { get; set; }

        // Số lần bị từ chối xin Undo trong trận này
        public int UndoRejections { get; set; }

        public bool IsDisconnected { get; set; }
    }
}
