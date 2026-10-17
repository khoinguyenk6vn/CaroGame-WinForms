using CaroGame.BLL.Models;

namespace CaroGame.BLL.Game
{
    // Đổi các giá trị trong game ra chữ tiếng Việt, dùng chung cho màn kết quả, lịch sử...
    public static class GameTexts
    {
        public static string GetModeText(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.PvE:
                    return "PvE";
                case GameMode.PvPOffline:
                    return "PvP Offline";
                default:
                    return "PvP Online";
            }
        }

        public static string GetResultText(MatchResult result)
        {
            switch (result)
            {
                case MatchResult.Win:
                    return "Thắng";
                case MatchResult.Lose:
                    return "Thua";
                default:
                    return "Hòa";
            }
        }

        public static string GetReasonText(EndReason reason)
        {
            switch (reason)
            {
                case EndReason.FiveInRow:
                    return "Đủ 5 quân";
                case EndReason.Timeout:
                    return "Hết giờ";
                case EndReason.Forfeit:
                    return "Đầu hàng";
                case EndReason.Disconnect:
                    return "Mất kết nối";
                default:
                    return "Bàn cờ đầy";
            }
        }
    }
}
