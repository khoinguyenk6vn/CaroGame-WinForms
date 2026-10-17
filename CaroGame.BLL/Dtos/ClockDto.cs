namespace CaroGame.BLL.Dtos
{
    // Số giây còn lại của 2 người, server gửi mỗi giây
    public class ClockDto
    {
        public int SecondsLeftX { get; set; }
        public int SecondsLeftO { get; set; }
    }
}
