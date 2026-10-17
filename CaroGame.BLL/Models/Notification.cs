namespace CaroGame.BLL.Models
{
    // Một tin cần gửi cho 1 client: gửi tới kết nối nào, tên sự kiện gì, kèm dữ liệu gì
    public class Notification
    {
        public string ConnectionId { get; }
        public string Method { get; }
        public object Data { get; }

        public Notification(string connectionId, string method, object data)
        {
            ConnectionId = connectionId;
            Method = method;
            Data = data;
        }
    }
}
