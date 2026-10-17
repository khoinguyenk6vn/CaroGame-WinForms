
namespace CaroGame.BLL.Game
{
    // Bot PvE: chấm điểm từng ô trống rồi chọn ô có điểm cao nhất.
    // Chỉ cần đánh đúng luật, không cần mạnh.
    public class CaroBot
    {
        // PvE luôn áp dụng luật chặn 2 đầu
        private const bool BlockBothEndsRule = true;

        public (int Row, int Col) ChooseMove(Board board, Stone botStone)
        {
            Stone playerStone = GameRules.Opponent(botStone);

            // 1. Có ô đánh vào là thắng ngay thì đánh luôn
            var winningCell = FindWinningCell(board, botStone);
            if (winningCell != null)
            {
                return winningCell.Value;
            }

            // 2. Người chơi đánh vào ô nào là thắng thì chặn ô đó
            var blockingCell = FindWinningCell(board, playerStone);
            if (blockingCell != null)
            {
                return blockingCell.Value;
            }

            // 3. Còn lại: chọn ô có điểm cao nhất
            return FindBestScoredCell(board, botStone, playerStone);
        }

        // Thử đặt quân vào từng ô trống, kiểm tra thắng bằng đúng luật của game, rồi gỡ quân ra
        private (int Row, int Col)? FindWinningCell(Board board, Stone stone)
        {
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    if (!board.IsEmpty(row, col))
                    {
                        continue;
                    }

                    board.PlaceStone(row, col, stone);
                    bool isWinning = GameRules.IsWinningMove(board, row, col, BlockBothEndsRule);
                    board.UndoLastMove();

                    if (isWinning)
                    {
                        return (row, col);
                    }
                }
            }

            return null;
        }

        private (int Row, int Col) FindBestScoredCell(Board board, Stone botStone, Stone playerStone)
        {
            int bestScore = -1;
            int bestRow = 0;
            int bestCol = 0;

            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    if (!board.IsEmpty(row, col))
                    {
                        continue;
                    }

                    // Điểm tấn công (hàng của bot) + điểm phòng thủ (chặn hàng của người chơi)
                    int score = ScoreCell(board, row, col, botStone) + ScoreCell(board, row, col, playerStone);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestRow = row;
                        bestCol = col;
                    }
                }
            }

            return (bestRow, bestCol);
        }

        // Chấm điểm 1 ô trống nếu đặt quân stone vào đó, cộng dồn cả 4 hướng
        private int ScoreCell(Board board, int row, int col, Stone stone)
        {
            int total = 0;

            foreach (var direction in GameRules.Directions)
            {
                int forward = GameRules.CountSameStones(board, row, col, direction.Row, direction.Col, stone);
                int backward = GameRules.CountSameStones(board, row, col, -direction.Row, -direction.Col, stone);

                // Đầu hàng còn trống thì hàng còn phát triển được
                int openEnds = 0;
                if (IsEmptyCell(board, row + direction.Row * (forward + 1), col + direction.Col * (forward + 1)))
                {
                    openEnds++;
                }
                if (IsEmptyCell(board, row - direction.Row * (backward + 1), col - direction.Col * (backward + 1)))
                {
                    openEnds++;
                }

                int lineLength = forward + backward + 1;
                total += ScoreLine(lineLength, openEnds);
            }

            return total;
        }

        private bool IsEmptyCell(Board board, int row, int col)
        {
            return board.IsInside(row, col) && board.IsEmpty(row, col);
        }

        // Hàng càng dài và càng nhiều đầu trống thì điểm càng cao
        private int ScoreLine(int lineLength, int openEnds)
        {
            if (lineLength == 1 || openEnds == 0)
            {
                return 0;
            }

            if (lineLength == 2)
            {
                return openEnds == 2 ? 100 : 10;
            }

            if (lineLength == 3)
            {
                return openEnds == 2 ? 1000 : 100;
            }

            return openEnds == 2 ? 10000 : 1000;
        }
    }
}
