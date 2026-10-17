namespace CaroGame.BLL.Game
{
    // Đổi điểm ra bậc rank: 9 hạng, mỗi hạng 3 bậc I, II, III, mỗi bậc rộng 100 điểm, bắt đầu từ Sắt I = 1000
    public static class RankCalculator
    {
        private static readonly string[] TierNames =
        {
            "Sắt", "Đồng", "Bạc", "Vàng", "Bạch Kim", "Kim Cương", "Lục Bảo", "Trái Đất", "Mặt Trời"
        };

        private static readonly string[] LevelNames = { "I", "II", "III" };

        public static string GetRankName(int score)
        {
            // Số thứ tự bậc tính từ 0 (Sắt I). Bậc cuối là 26 (Mặt Trời III), từ 3600 trở lên vẫn là bậc cuối.
            int rankIndex = (score - 1000) / 100;
            if (rankIndex < 0)
            {
                rankIndex = 0;
            }
            if (rankIndex > 26)
            {
                rankIndex = 26;
            }

            return TierNames[rankIndex / 3] + " " + LevelNames[rankIndex % 3];
        }
    }
}
