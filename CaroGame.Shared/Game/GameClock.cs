namespace CaroGame.Shared.Game
{
    // Đồng hồ kiểu cờ vua: mỗi người có tổng 180 giây cho cả trận,
    // chỉ đồng hồ của người đang tới lượt bị trừ.
    public class GameClock
    {
        public const int SecondsPerPlayer = 180;

        public int SecondsLeftX { get; private set; }
        public int SecondsLeftO { get; private set; }

        // Quân đang bị tính giờ. Stone.None nghĩa là đồng hồ đang dừng.
        public Stone RunningFor { get; private set; }

        public GameClock()
        {
            Reset();
        }

        public void Reset()
        {
            SecondsLeftX = SecondsPerPlayer;
            SecondsLeftO = SecondsPerPlayer;
            RunningFor = Stone.None;
        }

        public void StartFor(Stone stone)
        {
            RunningFor = stone;
        }

        public void Stop()
        {
            RunningFor = Stone.None;
        }

        // Gọi mỗi giây một lần: trừ 1 giây của người đang bị tính giờ
        public void TickOneSecond()
        {
            if (RunningFor == Stone.X && SecondsLeftX > 0)
            {
                SecondsLeftX--;
            }
            else if (RunningFor == Stone.O && SecondsLeftO > 0)
            {
                SecondsLeftO--;
            }
        }

        public int GetSecondsLeft(Stone stone)
        {
            if (stone == Stone.X)
            {
                return SecondsLeftX;
            }
            return SecondsLeftO;
        }

        public bool IsTimeUp(Stone stone)
        {
            return GetSecondsLeft(stone) == 0;
        }
    }
}
