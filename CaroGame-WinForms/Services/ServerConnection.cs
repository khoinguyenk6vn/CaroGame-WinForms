using CaroGame.BLL.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Services
{
    // Giữ 1 kết nối SignalR duy nhất tới server cho cả app
    public static class ServerConnection
    {
        private static HubConnection? connection;

        // Địa chỉ server lấy từ clientsettings.json, ví dụ http://localhost:5023
        public static string ServerUrl
        {
            get { return ClientSettings.Load().ServerUrl.TrimEnd('/'); }
        }

        // Trả về kết nối tới server, chưa kết nối thì kết nối.
        public static async Task<HubConnection> ConnectAsync()
        {
            if (connection == null)
            {
                // WithAutomaticReconnect: mất mạng thì SignalR tự thử kết nối lại (sau 0, 2, 10, 30 giây)
                connection = new HubConnectionBuilder()
                    .WithUrl(ServerUrl + "/gamehub")
                    .WithAutomaticReconnect()
                    .Build();
                connection.Reconnected += Connection_Reconnected;
            }

            if (connection.State == HubConnectionState.Disconnected)
            {
                await connection.StartAsync();
            }

            return connection;
        }

        // Kết nối lại xong thì mã kết nối đã đổi: gửi token để server gắn lại phiên đăng nhập.
        // Server trả false nghĩa là phiên đã bị giải phóng, coi như đã đăng xuất.
        private static async Task Connection_Reconnected(string? newConnectionId)
        {
            if (!Session.IsLoggedIn || connection == null)
            {
                return;
            }

            bool sessionKept = await connection.InvokeAsync<bool>("Reconnect", Session.Token);
            if (!sessionKept)
            {
                Session.StartAsGuest();
            }
        }
    }
}
