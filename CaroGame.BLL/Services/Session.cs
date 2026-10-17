using CaroGame.BLL.Dtos;

namespace CaroGame.BLL.Services
{
    // Thông tin người đang chơi trên máy client, chỉ tồn tại trong lúc app chạy.
    // Guest không lưu gì vào CSDL.
    public static class Session
    {
        private const int DefaultAvatarCount = 20;

        private static readonly Random random = new Random();

        public static string PlayerName { get; private set; } = "";
        public static bool IsLoggedIn { get; private set; }
        public static string Token { get; private set; } = "";
        public static int Score { get; private set; }
        public static int DefaultAvatarIndex { get; private set; }

        // Rỗng nếu chưa upload avatar
        public static string AvatarPath { get; private set; } = "";

        // Guest được đặt tên tự sinh dạng Guest12345 và avatar mặc định ngẫu nhiên mỗi lần mở app
        public static void StartAsGuest()
        {
            PlayerName = "Guest" + random.Next(10000, 100000);
            IsLoggedIn = false;
            Token = "";
            Score = 0;
            DefaultAvatarIndex = random.Next(DefaultAvatarCount);
            AvatarPath = "";
        }

        public static void Login(LoginResult result)
        {
            PlayerName = result.Username;
            IsLoggedIn = true;
            Token = result.Token;
            Score = result.Score;
            DefaultAvatarIndex = result.DefaultAvatarIndex;
            AvatarPath = result.AvatarPath;
        }

        // Cập nhật lại sau khi xem Profile hoặc đổi avatar
        public static void UpdateProfile(ProfileDto profile)
        {
            Score = profile.Score;
            DefaultAvatarIndex = profile.DefaultAvatarIndex;
            AvatarPath = profile.AvatarPath;
        }

        public static void UpdateScore(int newScore)
        {
            Score = newScore;
        }
    }
}
