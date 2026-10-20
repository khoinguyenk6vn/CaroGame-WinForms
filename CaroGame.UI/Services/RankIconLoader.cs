using System.IO;
using CaroGame.BLL.Game;

namespace CaroGame.UI.Services
{
    // Lấy ảnh logo rank để hiện lên PictureBox hoặc DataGridView.
    // 27 ảnh nằm trong thư mục Ranks cạnh file .exe: rank_1.png (Sắt I) ... rank_27.png (Mặt Trời III).
    // Ảnh lấy từ logo rank của game Valorant (bản quyền của Riot Games, xem README).
    public static class RankIconLoader
    {
        // Trả về null nếu thiếu file ảnh (khi đó không hiện logo, game vẫn chạy bình thường)
        public static Image? Load(int score)
        {
            // Điểm -> số thứ tự bậc (1..27) do tầng BLL tính
            int rankNumber = RankCalculator.GetRankNumber(score);

            string path = Path.Combine(AppContext.BaseDirectory, "Ranks", "rank_" + rankNumber + ".png");
            if (!File.Exists(path))
            {
                return null;
            }
            return Image.FromFile(path);
        }
    }
}
