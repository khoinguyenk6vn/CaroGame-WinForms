namespace CaroGame.BLL.Dtos
{
    // Kết quả lời xin Undo, gửi cho người xin
    public class UndoAnswerDto
    {
        public bool Accepted { get; set; }
        public string Message { get; set; } = "";
    }
}
