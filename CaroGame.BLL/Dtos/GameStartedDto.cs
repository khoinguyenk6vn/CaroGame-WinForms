namespace CaroGame.BLL.Dtos
{
    // Server gửi khi đã chốt luật và trận bắt đầu (đồng hồ của X bắt đầu chạy)
    public class GameStartedDto
    {
        public bool BlockBothEndsRule { get; set; }
    }
}
