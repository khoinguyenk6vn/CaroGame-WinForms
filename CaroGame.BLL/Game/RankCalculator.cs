namespace CaroGame.BLL.Game
{
    // Đổi điểm ra bậc rank: 9 hạng, mỗi hạng 3 bậc I, II, III, mỗi bậc rộng 100 điểm, bắt đầu từ Sắt I = 1000
    public static class RankCalculator
    {
        public const int RankCount = 27;

        private static readonly string[] TierNames =
        {
            "Sắt", "Đồng", "Bạc", "Vàng", "Bạch Kim", "Kim Cương", "Lục Bảo", "Trái Đất", "Mặt Trời"
        };

        private static readonly string[] LevelNames = { "I", "II", "III" };

        // Số thứ tự bậc từ 1 (Sắt I) đến 27 (Mặt Trời III). Từ 3600 điểm trở lên vẫn là 27.
        // Client dùng số này để chọn ảnh logo rank (Ranks/rank_1.png ... rank_27.png).
        public static int GetRankNumber(int score)
        {
            int rankNumber = (score - 1000) / 100 + 1;
            if (rankNumber < 1)
            {
                rankNumber = 1;
            }
            if (rankNumber > RankCount)
            {
                rankNumber = RankCount;
            }
            return rankNumber;
        }

        public static string GetRankName(int score)
        {
            // Đổi về số thứ tự tính từ 0 để lấy tên hạng (chia 3) và tên bậc (chia lấy dư 3)
            int rankIndex = GetRankNumber(score) - 1;
            return TierNames[rankIndex / 3] + " " + LevelNames[rankIndex % 3];
        }
    }
}
