namespace CaroGame.BLL.Game
{
    // Luật chơi dùng chung cho Client (PvE, Offline) và Server (Online)
    public static class GameRules
    {
        public const int WinLength = 5;

        // 4 hướng cần kiểm tra: ngang, dọc, chéo xuống phải, chéo xuống trái.
        // Mỗi hướng chỉ cần 1 chiều, chiều ngược lại được đếm bằng cách đổi dấu.
        public static readonly (int Row, int Col)[] Directions =
        {
            (0, 1),
            (1, 0),
            (1, 1),
            (1, -1)
        };

        public static Stone Opponent(Stone stone)
        {
            return stone == Stone.X ? Stone.O : Stone.X;
        }

        public static bool IsValidMove(Board board, int row, int col)
        {
            return board.IsInside(row, col) && board.IsEmpty(row, col);
        }

        // Kiểm tra quân vừa đặt ở (row, col) có tạo thành hàng thắng không
        public static bool IsWinningMove(Board board, int row, int col, bool blockBothEndsRule)
        {
            Stone stone = board.GetStone(row, col);
            if (stone == Stone.None)
            {
                return false;
            }

            foreach (var direction in Directions)
            {
                if (IsWinningLine(board, row, col, direction.Row, direction.Col, blockBothEndsRule))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsWinningLine(Board board, int row, int col, int dRow, int dCol, bool blockBothEndsRule)
        {
            Stone stone = board.GetStone(row, col);
            int forward = CountSameStones(board, row, col, dRow, dCol, stone);
            int backward = CountSameStones(board, row, col, -dRow, -dCol, stone);

            // Từ 5 quân trở lên (kể cả 6+) đều tính là đủ dài
            int length = 1 + forward + backward;
            if (length < WinLength)
            {
                return false;
            }

            if (!blockBothEndsRule)
            {
                return true;
            }

            // Luật chặn 2 đầu: xét 2 ô nằm sát ngay 2 đầu của hàng.
            // Chỉ khi cả 2 ô đều là quân đối thủ thì hàng này không được tính thắng.
            bool firstEndBlocked = IsOpponentStone(board, row + dRow * (forward + 1), col + dCol * (forward + 1), stone);
            bool secondEndBlocked = IsOpponentStone(board, row - dRow * (backward + 1), col - dCol * (backward + 1), stone);
            return !(firstEndBlocked && secondEndBlocked);
        }

        // Đếm số quân cùng màu liên tiếp theo 1 hướng, không tính ô xuất phát
        public static int CountSameStones(Board board, int row, int col, int dRow, int dCol, Stone stone)
        {
            int count = 0;
            int r = row + dRow;
            int c = col + dCol;

            while (board.IsInside(r, c) && board.GetStone(r, c) == stone)
            {
                count++;
                r += dRow;
                c += dCol;
            }

            return count;
        }

        // Mép bàn cờ nằm ngoài bàn nên trả về false: mép KHÔNG tính là bị chặn
        private static bool IsOpponentStone(Board board, int row, int col, Stone stone)
        {
            return board.IsInside(row, col) && board.GetStone(row, col) == Opponent(stone);
        }
    }
}
