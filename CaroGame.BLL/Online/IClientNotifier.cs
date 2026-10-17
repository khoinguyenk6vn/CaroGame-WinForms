namespace CaroGame.BLL.Online
{
    // BLL không biết SignalR. Server cài đặt interface này để gửi tin tới client,
    // BLL chỉ việc gọi SendAsync.
    public interface IClientNotifier
    {
        Task SendAsync(string connectionId, string method, object data);
    }
}
