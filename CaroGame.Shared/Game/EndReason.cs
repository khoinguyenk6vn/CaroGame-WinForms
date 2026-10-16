namespace CaroGame.Shared.Game
{
    // Lý do kết thúc trận, cũng là giá trị sẽ lưu vào lịch sử đấu
    public enum EndReason
    {
        FiveInRow,
        Timeout,
        Forfeit,
        Disconnect,
        Draw
    }
}
