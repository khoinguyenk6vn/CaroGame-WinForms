using CaroGame.BLL.Online;

namespace CaroGame.Server
{
    // Chạy nền suốt thời gian server bật: mỗi giây gọi OnlineGameManager.TickAsync() một lần
    // (ghép trận, chạy đồng hồ, kiểm tra các hạn 15 giây và 30 giây)
    public class GameTickService : BackgroundService
    {
        private readonly OnlineGameManager gameManager;
        private readonly ILogger<GameTickService> logger;

        public GameTickService(OnlineGameManager gameManager, ILogger<GameTickService> logger)
        {
            this.gameManager = gameManager;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Lỗi ở 1 lần chạy thì ghi log rồi chạy tiếp, không để dừng cả vòng lặp
                try
                {
                    await gameManager.TickAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Loi khi chay TickAsync");
                }
            }
        }
    }
}
