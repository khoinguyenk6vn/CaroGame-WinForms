using CaroGame.BLL.Dtos;
using CaroGame.BLL.Models;

namespace CaroGame.BLL.Game
{
    // Trạng thái trận Online phía client. Client không tự quyết gì:
    // chỉ cập nhật theo tin server gửi về (nước đi, Undo, đồng hồ, kết quả).
    public class OnlineMatchState
    {
        public Board Board { get; } = new Board();
        public string XName { get; }
        public string OName { get; }
        public Stone MyStone { get; }
        public Stone CurrentTurn { get; private set; } = Stone.X;
        public int SecondsLeftX { get; private set; } = GameClock.SecondsPerPlayer;
        public int SecondsLeftO { get; private set; } = GameClock.SecondsPerPlayer;

        // false trong lúc đang hỏi luật chặn 2 đầu
        public bool IsStarted { get; private set; }
        public bool BlockBothEndsRule { get; private set; }
        public bool IsOver { get; private set; }

        public OnlineMatchState(MatchFoundDto info)
        {
            XName = info.XName;
            OName = info.OName;
            MyStone = info.MyStone;
        }

        public string GetPlayerName(Stone stone)
        {
            return stone == Stone.X ? XName : OName;
        }

        public void Start(GameStartedDto info)
        {
            IsStarted = true;
            BlockBothEndsRule = info.BlockBothEndsRule;
        }

        public void ApplyMove(MoveDto move)
        {
            Board.PlaceStone(move.Row, move.Col, move.Stone);
            CurrentTurn = move.NextTurn;
        }

        public void ApplyUndo(MoveDto move)
        {
            Board.UndoLastMove();
            CurrentTurn = move.NextTurn;
        }

        public void ApplyClock(ClockDto clock)
        {
            SecondsLeftX = clock.SecondsLeftX;
            SecondsLeftO = clock.SecondsLeftO;
        }

        public void Finish()
        {
            IsOver = true;
        }
    }
}
