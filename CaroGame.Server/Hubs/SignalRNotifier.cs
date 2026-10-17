using CaroGame.BLL.Online;
using Microsoft.AspNetCore.SignalR;

namespace CaroGame.Server.Hubs
{
    // Cài đặt IClientNotifier của BLL bằng SignalR: gửi tin tới đúng 1 client theo mã kết nối
    public class SignalRNotifier : IClientNotifier
    {
        private readonly IHubContext<GameHub> hubContext;

        public SignalRNotifier(IHubContext<GameHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task SendAsync(string connectionId, string method, object data)
        {
            await hubContext.Clients.Client(connectionId).SendAsync(method, data);
        }
    }
}
