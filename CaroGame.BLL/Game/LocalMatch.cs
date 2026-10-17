
using CaroGame.BLL.Dtos;
using CaroGame.BLL.Models;

namespace CaroGame.BLL.Game
{
    // Một trận PvE hoặc PvP Offline, chạy hoàn toàn trên máy, không cần server.
    // Form chỉ gọi các hàm ở đây rồi vẽ lại theo trạng thái trận.
    public class LocalMatch
    {
        // PvE và Offline luôn áp dụng luật chặn 2 đầu
        private const bool BlockBothEndsRule = true;

        // Người chơi 1 luôn cầm X và đi trước. Ở PvE, bot cầm O.
        public const Stone Player1Stone = Stone.X;
        public const Stone BotStone = Stone.O;

        private readonly CaroBot bot = new CaroBot();

        public GameMode Mode { get; }
        public Board Board { get; } = new Board();
        public GameClock Clock { get; } = new GameClock();
        public Stone CurrentTurn { get; private set; }
        public bool IsOver { get; private set; }

        // Stone.None nghĩa là hòa
        public Stone Winner { get; private set; }
        public EndReason EndReason { get; private set; }

        public string Player1Name { get; }
        public string Player2Name { get; }

        // Người chơi 2: PvE là "Bot", Offline là "Người chơi 2"
        public LocalMatch(GameMode mode, string player1Name)
        {
            Mode = mode;
            Player1Name = player1Name;
            Player2Name = mode == GameMode.PvE ? "Bot" : "Người chơi 2";
            Start();
        }

        public string GetPlayerName(Stone stone)
        {
            return stone == Player1Stone ? Player1Name : Player2Name;
        }

        // Bắt đầu (hoặc chơi lại) một ván mới. Đồng hồ của X chạy ngay.
        public void Start()
        {
            Board.Clear();
            Clock.Reset();
            CurrentTurn = Stone.X;
            IsOver = false;
            Winner = Stone.None;
            Clock.StartFor(Stone.X);
        }

        // PvE: đang tới lượt bot
        public bool IsBotTurn => Mode == GameMode.PvE && !IsOver && CurrentTurn == BotStone;

        // Người chơi đánh vào ô (row, col). Trả về false nếu nước đi không hợp lệ
        // (ô ngoài bàn, ô đã có quân, trận đã kết thúc, hoặc đang tới lượt bot).
        public bool PlayMove(int row, int col)
        {
            if (IsOver || IsBotTurn || !GameRules.IsValidMove(Board, row, col))
            {
                return false;
            }

            PlaceStone(row, col);
            return true;
        }

        // PvE: bot chọn ô và đánh. Trả về false nếu lúc này không phải lượt bot
        // (ví dụ người chơi vừa đầu hàng trong lúc bot đang chờ).
        public bool PlayBotMove()
        {
            if (!IsBotTurn)
            {
                return false;
            }

            var botMove = bot.ChooseMove(Board, BotStone);
            PlaceStone(botMove.Row, botMove.Col);
            return true;
        }

        // Đặt quân của người đang tới lượt, rồi kiểm tra thắng, hòa, hoặc đổi lượt
        private void PlaceStone(int row, int col)
        {
            Board.PlaceStone(row, col, CurrentTurn);

            if (GameRules.IsWinningMove(Board, row, col, BlockBothEndsRule))
            {
                Finish(CurrentTurn, EndReason.FiveInRow);
                return;
            }

            if (Board.IsFull)
            {
                Finish(Stone.None, EndReason.Draw);
                return;
            }

            CurrentTurn = GameRules.Opponent(CurrentTurn);
            Clock.StartFor(CurrentTurn);
        }

        // Form gọi hàm này mỗi giây. Trả về true nếu vừa có người hết giờ.
        public bool TickClock()
        {
            // PvE không tính giờ: đồng hồ vẫn hiện trên màn hình nhưng đứng yên
            if (IsOver || Mode == GameMode.PvE)
            {
                return false;
            }

            Clock.TickOneSecond();

            if (Clock.IsTimeUp(CurrentTurn))
            {
                Finish(GameRules.Opponent(CurrentTurn), EndReason.Timeout);
                return true;
            }

            return false;
        }

        // Undo không cần hỏi, không hoàn lại thời gian đã dùng.
        // PvE lùi 2 nước (nước của bot + nước của mình), Offline lùi 1 nước.
        public bool Undo()
        {
            int movesToUndo = Mode == GameMode.PvE ? 2 : 1;
            if (IsOver || IsBotTurn || Board.MoveCount < movesToUndo)
            {
                return false;
            }

            Move? undoneMove = null;
            for (int i = 0; i < movesToUndo; i++)
            {
                undoneMove = Board.UndoLastMove();
            }

            // Trả lượt lại cho người vừa đi (ở PvE đó luôn là người chơi)
            CurrentTurn = undoneMove!.Stone;
            Clock.StartFor(CurrentTurn);
            return true;
        }

        // FF: PvE là người chơi đầu hàng, Offline là người đang tới lượt đầu hàng
        public void Surrender()
        {
            if (IsOver)
            {
                return;
            }

            Stone loser = Mode == GameMode.PvE ? Player1Stone : CurrentTurn;
            Finish(GameRules.Opponent(loser), EndReason.Forfeit);
        }

        // Kết quả trận nhìn từ phía người chơi 1 (người đang dùng máy), để gửi lên server lưu lịch sử
        public LocalMatchResultDto CreateResultForPlayer1()
        {
            MatchResult result = MatchResult.Draw;
            if (Winner == Player1Stone)
            {
                result = MatchResult.Win;
            }
            else if (Winner != Stone.None)
            {
                result = MatchResult.Lose;
            }

            return new LocalMatchResultDto
            {
                Mode = Mode,
                Result = result,
                EndReason = EndReason,
                MoveCount = Board.MoveCount
            };
        }

        private void Finish(Stone winner, EndReason reason)
        {
            Clock.Stop();
            IsOver = true;
            Winner = winner;
            EndReason = reason;
        }
    }
}
