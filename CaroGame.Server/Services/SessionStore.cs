namespace CaroGame.Server.Services
{
    // Danh sách người đang đăng nhập, chỉ lưu trong RAM (không lưu CSDL).
    // Server khởi động lại thì danh sách trống, mọi người phải đăng nhập lại.
    public static class SessionStore
    {
        private static readonly List<OnlineUser> onlineUsers = new List<OnlineUser>();

        // Nhiều người đăng nhập cùng lúc sẽ chạy trên nhiều luồng khác nhau,
        // nên mọi thao tác với danh sách phải nằm trong lock để không bị ghi đè lẫn nhau
        private static readonly object lockObject = new object();

        // Thêm người dùng vào danh sách online và sinh session token.
        // Trả về null nếu tài khoản đang đăng nhập ở nơi khác.
        public static string? TryAdd(int userId, string username, string connectionId)
        {
            lock (lockObject)
            {
                foreach (OnlineUser onlineUser in onlineUsers)
                {
                    if (onlineUser.UserId == userId)
                    {
                        return null;
                    }
                }

                var newUser = new OnlineUser
                {
                    UserId = userId,
                    Username = username,
                    ConnectionId = connectionId,
                    Token = Guid.NewGuid().ToString()
                };
                onlineUsers.Add(newUser);
                return newUser.Token;
            }
        }

        // Ngoài trận: mất kết nối là giải phóng phiên ngay, tài khoản đăng nhập lại được ở máy khác
        public static void RemoveByConnection(string connectionId)
        {
            lock (lockObject)
            {
                onlineUsers.RemoveAll(u => u.ConnectionId == connectionId);
            }
        }
    }
}
