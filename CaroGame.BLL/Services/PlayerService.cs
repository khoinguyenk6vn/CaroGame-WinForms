using CaroGame.BLL.Dtos;
using CaroGame.BLL.Game;
using CaroGame.DAL;
using CaroGame.DAL.Entities;

namespace CaroGame.BLL.Services
{
    // Nghiệp vụ dữ liệu người chơi: Profile, lịch sử đấu, bảng xếp hạng, lưu kết quả trận PvE/Offline.
    // Người gọi được nhận ra qua mã kết nối SignalR (phải đăng nhập rồi mới dùng được).
    public class PlayerService
    {
        private const int LeaderboardSize = 50;

        private readonly UserDAL userDal;
        private readonly MatchHistoryDAL historyDal;

        public PlayerService(UserDAL userDal, MatchHistoryDAL historyDal)
        {
            this.userDal = userDal;
            this.historyDal = historyDal;
        }

        // Trả về null nếu chưa đăng nhập
        public async Task<ProfileDto?> GetProfileAsync(string connectionId)
        {
            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return null;
            }

            User? user = await userDal.GetByIdAsync(onlineUser.UserId);
            if (user == null)
            {
                return null;
            }

            return new ProfileDto
            {
                Username = user.Username,
                Score = user.Score,
                RankName = RankCalculator.GetRankName(user.Score),
                DefaultAvatarIndex = user.DefaultAvatarIndex,
                AvatarPath = user.AvatarPath ?? ""
            };
        }

        public async Task<List<HistoryItemDto>> GetHistoryAsync(string connectionId)
        {
            var items = new List<HistoryItemDto>();

            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return items;
            }

            List<MatchHistory> histories = await historyDal.GetByUserAsync(onlineUser.UserId);
            foreach (MatchHistory history in histories)
            {
                GameMode mode = Enum.Parse<GameMode>(history.Mode);
                items.Add(new HistoryItemDto
                {
                    PlayedAt = history.PlayedAt.ToString("dd/MM/yyyy HH:mm"),
                    Mode = GameTexts.GetModeText(mode),
                    Opponent = GetOpponentName(mode, history),
                    Result = GameTexts.GetResultText(Enum.Parse<MatchResult>(history.Result)),
                    Reason = GameTexts.GetReasonText(Enum.Parse<EndReason>(history.EndReason)),
                    MoveCount = history.MoveCount,
                    ScoreChange = FormatScoreChange(history.ScoreChange)
                });
            }

            return items;
        }

        private string GetOpponentName(GameMode mode, MatchHistory history)
        {
            if (mode == GameMode.PvE)
            {
                return "Bot";
            }
            if (mode == GameMode.PvPOffline)
            {
                return "Người chơi 2";
            }
            return history.Opponent != null ? history.Opponent.Username : "";
        }

        // Trận Online hiện số điểm cộng/trừ, PvE và Offline để trống
        private string FormatScoreChange(int? scoreChange)
        {
            if (scoreChange == null)
            {
                return "";
            }
            return scoreChange.Value > 0 ? "+" + scoreChange.Value : scoreChange.Value.ToString();
        }

        // Top 50, dòng cuối luôn là thứ hạng của chính người xem
        public async Task<List<LeaderboardRowDto>> GetLeaderboardAsync(string connectionId)
        {
            var rows = new List<LeaderboardRowDto>();

            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return rows;
            }

            List<UserRanking> ranking = await userDal.GetRankingAsync();
            LeaderboardRowDto? myRow = null;

            for (int i = 0; i < ranking.Count; i++)
            {
                var row = new LeaderboardRowDto
                {
                    Position = i + 1,
                    Username = ranking[i].Username,
                    Score = ranking[i].Score,
                    RankName = RankCalculator.GetRankName(ranking[i].Score)
                };

                if (i < LeaderboardSize)
                {
                    rows.Add(row);
                }
                if (ranking[i].UserId == onlineUser.UserId)
                {
                    myRow = row;
                }
            }

            if (myRow != null)
            {
                rows.Add(myRow);
            }

            return rows;
        }

        // Lưu kết quả trận PvE/Offline. Không có điểm, chỉ để hiện trong lịch sử.
        public async Task SaveLocalMatchAsync(string connectionId, LocalMatchResultDto result)
        {
            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return;
            }

            // Kết quả trận Online do server tự lưu, client không được gửi
            if (result.Mode == GameMode.PvPOnline)
            {
                return;
            }

            await historyDal.AddAsync(new MatchHistory
            {
                UserId = onlineUser.UserId,
                OpponentId = null,
                Mode = result.Mode.ToString(),
                Result = result.Result.ToString(),
                EndReason = result.EndReason.ToString(),
                BlockBothEndsRule = true,
                MoveCount = result.MoveCount,
                ScoreChange = null,
                PlayedAt = DateTime.Now
            });
        }
    }
}
