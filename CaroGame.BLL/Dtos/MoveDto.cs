using CaroGame.BLL.Game;

namespace CaroGame.BLL.Dtos
{
    // Một nước vừa được đánh (hoặc vừa bị Undo) trong trận Online
    public class MoveDto
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Stone Stone { get; set; }

        // Lượt tiếp theo là của quân nào
        public Stone NextTurn { get; set; }
    }
}
