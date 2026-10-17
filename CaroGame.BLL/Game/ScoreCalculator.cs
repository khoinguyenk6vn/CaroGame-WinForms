namespace CaroGame.BLL.Game
{
    // Tính điểm rank sau trận Online. Đây là hệ điểm cố định theo số nước,
    // không phụ thuộc điểm đối thủ (không phải công thức Elo chuẩn).
    public static class ScoreCalculator
    {
        public const int MinimumScore = 1000;
        public const int ForfeitPenalty = 100;

        // n = số quân trên bàn lúc kết thúc
        public static int GetWinPoints(int n)
        {
            if (n <= 25)
            {
                return 50;
            }
            return Math.Max(35, 50 - (n - 25) / 2);
        }

        public static int GetLosePoints(int n)
        {
            if (n <= 25)
            {
                return 50;
            }
            return Math.Max(25, 50 - (n - 25) / 2);
        }

        // Điểm cộng (số dương) hoặc trừ (số âm) cho 1 người chơi
        public static int GetScoreDelta(MatchResult result, EndReason reason, int n)
        {
            if (result == MatchResult.Draw)
            {
                return 0;
            }

            // Thắng vì bất kỳ lý do gì (kể cả đối thủ FF, hết giờ, mất kết nối) đều tính theo n
            if (result == MatchResult.Win)
            {
                return GetWinPoints(n);
            }

            // Thua do đầu hàng hoặc mất kết nối quá 30 giây: trừ 100
            if (reason == EndReason.Forfeit || reason == EndReason.Disconnect)
            {
                return -ForfeitPenalty;
            }

            return -GetLosePoints(n);
        }

        // Điểm không bao giờ xuống dưới 1000
        public static int ApplyMinimum(int score)
        {
            return Math.Max(MinimumScore, score);
        }
    }
}
