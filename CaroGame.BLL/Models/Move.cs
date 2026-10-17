namespace CaroGame.BLL.Models
{
    // Một nước đi: quân nào được đặt ở ô nào
    public class Move
    {
        public int Row { get; }
        public int Col { get; }
        public Stone Stone { get; }

        public Move(int row, int col, Stone stone)
        {
            Row = row;
            Col = col;
            Stone = stone;
        }
    }
}
