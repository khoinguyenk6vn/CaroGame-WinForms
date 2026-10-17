namespace CaroGame.BLL.Services
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

        // Tìm người đang đăng nhập theo mã kết nối SignalR, null nếu chưa đăng nhập
        public static OnlineUser? FindByConnection(string connectionId)
        {
            lock (lockObject)
            {
                return onlineUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
            }
        }

        public static OnlineUser? FindByToken(string token)
        {
            lock (lockObject)
            {
                return onlineUsers.FirstOrDefault(u => u.Token == token);
            }
        }

        // SignalR tự kết nối lại thì mã kết nối đổi, client gửi token để gắn lại phiên với mã mới
        public static void UpdateConnection(string token, string newConnectionId)
        {
            lock (lockObject)
            {
                OnlineUser? user = onlineUsers.FirstOrDefault(u => u.Token == token);
                if (user != null)
                {
                    user.ConnectionId = newConnectionId;
                }
            }
        }

        public static void RemoveByConnection(string connectionId)
        {
            lock (lockObject)
            {
                onlineUsers.RemoveAll(u => u.ConnectionId == connectionId);
            }
        }

        public static void RemoveByUserId(int userId)
        {
            lock (lockObject)
            {
                onlineUsers.RemoveAll(u => u.UserId == userId);
            }
        }
    }
}
