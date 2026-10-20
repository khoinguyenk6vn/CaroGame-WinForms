using CaroGame.BLL.Models;

namespace CaroGame.BLL.Game
{
    // Bàn cờ 20x20: lưu quân trên từng ô và danh sách nước đã đánh (để Undo)
    public class Board
    {
        public const int Size = 20;

        private readonly Stone[,] cells = new Stone[Size, Size];
        private readonly List<Move> moves = new List<Move>();

        public int MoveCount
        {
            get { return moves.Count; }
        }

        public bool IsFull
        {
            get { return moves.Count == Size * Size; }
        }

        // Nước đi cuối cùng, null nếu bàn cờ còn trống
        public Move? LastMove
        {
            get
            {
                if (moves.Count == 0)
                {
                    return null;
                }
                return moves[moves.Count - 1];
            }
        }

        public Stone GetStone(int row, int col)
        {
            return cells[row, col];
        }

        public bool IsInside(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size;
        }

        public bool IsEmpty(int row, int col)
        {
            return cells[row, col] == Stone.None;
        }

        public void PlaceStone(int row, int col, Stone stone)
        {
            cells[row, col] = stone;
            moves.Add(new Move(row, col, stone));
        }

        // Gỡ nước đi cuối cùng ra khỏi bàn cờ. Trả về nước vừa gỡ, hoặc null nếu bàn trống.
        public Move? UndoLastMove()
        {
            Move? lastMove = LastMove;
            if (lastMove == null)
            {
                return null;
            }

            cells[lastMove.Row, lastMove.Col] = Stone.None;
            moves.RemoveAt(moves.Count - 1);
            return lastMove;
        }

        public void Clear()
        {
            Array.Clear(cells);
            moves.Clear();
        }
    }
}
