using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Services
{
    // Giữ 1 kết nối SignalR duy nhất tới server cho cả app
    public static class ServerConnection
    {
        private static HubConnection? connection;

        // Trả về kết nối tới server, chưa kết nối thì kết nối.
        // Địa chỉ server lấy từ clientsettings.json, ví dụ http://localhost:5023
        public static async Task<HubConnection> ConnectAsync()
        {
            if (connection == null)
            {
                string serverUrl = ClientSettings.Load().ServerUrl;
                connection = new HubConnectionBuilder()
                    .WithUrl(serverUrl.TrimEnd('/') + "/gamehub")
                    .Build();
            }

            if (connection.State == HubConnectionState.Disconnected)
            {
                await connection.StartAsync();
            }

            return connection;
        }
    }
}
