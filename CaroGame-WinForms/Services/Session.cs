namespace CaroGame_WinForms.Services
{
    // Thông tin người đang chơi trên máy này, chỉ tồn tại trong lúc app chạy.
    // Guest không lưu gì vào CSDL.
    public static class Session
    {
        private static readonly Random random = new Random();

        public static string PlayerName { get; private set; } = "";
        public static bool IsLoggedIn { get; private set; }
        public static string Token { get; private set; } = "";
        public static int Score { get; private set; }

        // Guest được đặt tên tự sinh dạng Guest12345
        public static void StartAsGuest()
        {
            PlayerName = "Guest" + random.Next(10000, 100000);
            IsLoggedIn = false;
            Token = "";
            Score = 0;
        }

        public static void Login(string username, string token, int score)
        {
            PlayerName = username;
            IsLoggedIn = true;
            Token = token;
            Score = score;
        }
    }
}
