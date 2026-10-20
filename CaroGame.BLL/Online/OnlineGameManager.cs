using CaroGame.BLL.Dtos;
using CaroGame.BLL.Game;
using CaroGame.BLL.Services;
using CaroGame.DAL.Entities;
using CaroGame.DAL.DataAccess;
using CaroGame.BLL.Models;

namespace CaroGame.BLL.Online
{
    // Quản lý PvP Online trên server: hàng chờ ghép trận và các trận đang chơi.
    // Chỉ có 1 đối tượng duy nhất trong suốt thời gian server chạy. Trận chỉ nằm trong RAM,
    // server khởi động lại thì các trận đang chơi bị hủy, không tính điểm.
    public class OnlineGameManager
    {
        private const int StartScoreGap = 100;
        private const int ScoreGapStep = 100;
        private const int SecondsPerStep = 10;
        private const int MatchAnyoneAfterSeconds = 60;

        private readonly List<WaitingPlayer> queue = new List<WaitingPlayer>();
        private readonly List<OnlineMatch> matches = new List<OnlineMatch>();

        // Khóa cho 2 danh sách trên (mỗi trận có lock riêng bên trong OnlineMatch)
        private readonly object lockObject = new object();

        private readonly Random random = new Random();
        private readonly IClientNotifier notifier;
        private readonly UserDAL userDal;
        private readonly MatchHistoryDAL historyDal;

        public OnlineGameManager(IClientNotifier notifier, UserDAL userDal, MatchHistoryDAL historyDal)
        {
            this.notifier = notifier;
            this.userDal = userDal;
            this.historyDal = historyDal;
        }

        // ===================== Ghép trận =====================

        // Trả về "" nếu đã vào hàng chờ, ngược lại trả về lý do
        public async Task<string> JoinQueueAsync(string connectionId)
        {
            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return "Bạn chưa đăng nhập.";
            }

            // Lấy điểm mới nhất trong CSDL để ghép trận
            User? user = await userDal.GetByIdAsync(onlineUser.UserId);
            if (user == null)
            {
                return "Tài khoản không tồn tại.";
            }

            List<OnlineMatch> newMatches;
            lock (lockObject)
            {
                if (queue.Any(w => w.UserId == user.Id) || matches.Any(m => m.HasPlayer(user.Id)))
                {
                    return "Bạn đang tìm trận hoặc đang trong trận.";
                }

                queue.Add(new WaitingPlayer
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Score = user.Score,
                    DefaultAvatarIndex = user.DefaultAvatarIndex,
                    AvatarPath = user.AvatarPath ?? "",
                    ConnectionId = connectionId,
                    SecondsWaited = 0
                });
                newMatches = MatchWaitingPlayers();
            }

            await SendMatchFoundAsync(newMatches);
            return "";
        }

        public async Task LeaveQueueAsync(string connectionId)
        {
            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return;
            }

            lock (lockObject)
            {
                queue.RemoveAll(w => w.UserId == onlineUser.UserId);
            }

            // Người chơi bấm "Dừng tìm" đúng lúc server vừa ghép được trận (tin MatchFound chưa tới máy họ):
            // hủy trận đó. Trận còn đang hỏi luật nên Surrender sẽ hủy trận, không ai bị trừ điểm,
            // đối thủ nhận "MatchCanceled" và quay về màn tìm trận.
            OnlineMatch? match = FindMatchOfUser(onlineUser.UserId);
            if (match != null && match.Phase == MatchPhase.VotingRule)
            {
                match.Surrender(onlineUser.UserId);
                await AfterActionAsync(match);
            }
        }

        // Ghép các cặp trong hàng chờ. Người chờ lâu nhất đứng đầu danh sách nên được ưu tiên ghép trước,
        // và được ghép với người có điểm gần mình nhất trong khoảng cho phép. Gọi trong lock.
        private List<OnlineMatch> MatchWaitingPlayers()
        {
            var newMatches = new List<OnlineMatch>();

            bool foundPair = true;
            while (foundPair)
            {
                foundPair = false;

                foreach (WaitingPlayer player in queue)
                {
                    WaitingPlayer? opponent = FindClosestOpponent(player);
                    if (opponent != null)
                    {
                        queue.Remove(player);
                        queue.Remove(opponent);
                        newMatches.Add(CreateMatch(player, opponent));
                        foundPair = true;
                        break;
                    }
                }
            }

            return newMatches;
        }

        private WaitingPlayer? FindClosestOpponent(WaitingPlayer player)
        {
            WaitingPlayer? best = null;
            int bestGap = int.MaxValue;
            int allowedGap = GetAllowedScoreGap(player);

            foreach (WaitingPlayer other in queue)
            {
                if (other == player)
                {
                    continue;
                }

                int gap = Math.Abs(other.Score - player.Score);
                if (gap <= allowedGap && gap < bestGap)
                {
                    best = other;
                    bestGap = gap;
                }
            }

            return best;
        }

        // Ban đầu tìm trong ±100 điểm, mỗi 10 giây mở rộng thêm ±100, sau 60 giây ghép với bất kỳ ai
        private int GetAllowedScoreGap(WaitingPlayer player)
        {
            if (player.SecondsWaited >= MatchAnyoneAfterSeconds)
            {
                return int.MaxValue;
            }
            return StartScoreGap + ScoreGapStep * (player.SecondsWaited / SecondsPerStep);
        }

        // Server chọn ngẫu nhiên ai cầm X. Gọi trong lock.
        private OnlineMatch CreateMatch(WaitingPlayer first, WaitingPlayer second)
        {
            WaitingPlayer playerX = first;
            WaitingPlayer playerO = second;
            if (random.Next(2) == 1)
            {
                playerX = second;
                playerO = first;
            }

            var match = new OnlineMatch(ToOnlinePlayer(playerX), ToOnlinePlayer(playerO));
            matches.Add(match);
            return match;
        }

        private OnlinePlayer ToOnlinePlayer(WaitingPlayer waitingPlayer)
        {
            return new OnlinePlayer
            {
                UserId = waitingPlayer.UserId,
                Username = waitingPlayer.Username,
                Score = waitingPlayer.Score,
                DefaultAvatarIndex = waitingPlayer.DefaultAvatarIndex,
                AvatarPath = waitingPlayer.AvatarPath,
                ConnectionId = waitingPlayer.ConnectionId
            };
        }

        private async Task SendMatchFoundAsync(List<OnlineMatch> newMatches)
        {
            foreach (OnlineMatch match in newMatches)
            {
                await SendAllAsync(match.TakeNotifications());
            }
        }

        // ===================== Thao tác trong trận =====================

        public async Task VoteRuleAsync(string connectionId, bool agree)
        {
            OnlineMatch? match = FindMatchOfConnection(connectionId, out int userId);
            if (match == null)
            {
                return;
            }

            match.VoteRule(userId, agree);
            await AfterActionAsync(match);
        }

        public async Task PlayMoveAsync(string connectionId, int row, int col)
        {
            OnlineMatch? match = FindMatchOfConnection(connectionId, out int userId);
            if (match == null)
            {
                return;
            }

            match.PlayMove(userId, row, col);
            await AfterActionAsync(match);
        }

        // Trả về "" nếu đã gửi lời xin, ngược lại trả về lý do không xin được
        public async Task<string> RequestUndoAsync(string connectionId)
        {
            OnlineMatch? match = FindMatchOfConnection(connectionId, out int userId);
            if (match == null)
            {
                return "Bạn không ở trong trận nào.";
            }

            string error = match.RequestUndo(userId);
            await AfterActionAsync(match);
            return error;
        }

        public async Task AnswerUndoAsync(string connectionId, bool accept)
        {
            OnlineMatch? match = FindMatchOfConnection(connectionId, out int userId);
            if (match == null)
            {
                return;
            }

            match.AnswerUndo(userId, accept);
            await AfterActionAsync(match);
        }

        public async Task SendChatAsync(string connectionId, string text)
        {
            OnlineMatch? match = FindMatchOfConnection(connectionId, out int userId);
            if (match == null)
            {
                return;
            }

            match.SendChat(userId, text);
            await AfterActionAsync(match);
        }

        public async Task SurrenderAsync(string connectionId)
        {
            OnlineMatch? match = FindMatchOfConnection(connectionId, out int userId);
            if (match == null)
            {
                return;
            }

            match.Surrender(userId);
            await AfterActionAsync(match);
        }

        // ===================== Mất kết nối =====================

        // Trả về true nếu người này đang trong trận và trận chờ họ kết nối lại (phải giữ phiên đăng nhập)
        public async Task<bool> HandleDisconnectAsync(string connectionId)
        {
            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return false;
            }

            lock (lockObject)
            {
                queue.RemoveAll(w => w.UserId == onlineUser.UserId);
            }

            OnlineMatch? match = FindMatchOfUser(onlineUser.UserId);
            if (match == null)
            {
                return false;
            }

            bool waitForReconnect = match.Disconnect(onlineUser.UserId);
            await AfterActionAsync(match);
            return waitForReconnect;
        }

        public async Task HandleReconnectAsync(int userId, string newConnectionId)
        {
            OnlineMatch? match = FindMatchOfUser(userId);
            if (match == null)
            {
                return;
            }

            match.Reconnect(userId, newConnectionId);
            await AfterActionAsync(match);
        }

        // ===================== Mỗi giây =====================

        // Server gọi hàm này mỗi giây: tăng thời gian chờ, ghép trận, chạy đồng hồ và các hạn 15/30 giây của từng trận
        public async Task TickAsync()
        {
            List<OnlineMatch> newMatches;
            List<OnlineMatch> runningMatches;
            List<WaitingPlayer> waitingPlayers;

            lock (lockObject)
            {
                foreach (WaitingPlayer player in queue)
                {
                    player.SecondsWaited++;
                }

                newMatches = MatchWaitingPlayers();
                runningMatches = new List<OnlineMatch>(matches);
                waitingPlayers = new List<WaitingPlayer>(queue);
            }

            // Báo thời gian đã chờ cho những người vẫn đang tìm trận
            foreach (WaitingPlayer player in waitingPlayers)
            {
                await notifier.SendAsync(player.ConnectionId, "SearchTime", player.SecondsWaited);
            }

            await SendMatchFoundAsync(newMatches);

            foreach (OnlineMatch match in runningMatches)
            {
                match.Tick();
                await AfterActionAsync(match);
            }
        }

        // ===================== Kết thúc trận =====================

        // Sau mỗi thao tác: gửi các tin đang chờ, nếu trận vừa kết thúc thì tính điểm và lưu kết quả
        private async Task AfterActionAsync(OnlineMatch match)
        {
            await SendAllAsync(match.TakeNotifications());

            if (match.Phase != MatchPhase.Finished)
            {
                return;
            }

            // Chỉ nơi nào xóa được trận khỏi danh sách mới xử lý kết quả, để không lưu 2 lần
            bool removed;
            lock (lockObject)
            {
                removed = matches.Remove(match);
            }

            if (removed && !match.IsCanceled)
            {
                int moveCount = match.Board.MoveCount;
                await SaveResultAsync(match, match.PlayerX, match.PlayerO, moveCount);
                await SaveResultAsync(match, match.PlayerO, match.PlayerX, moveCount);
            }
        }

        // Tính điểm, lưu điểm mới và lịch sử cho 1 người, rồi báo kết quả cho người đó
        private async Task SaveResultAsync(OnlineMatch match, OnlinePlayer player, OnlinePlayer opponent, int moveCount)
        {
            MatchResult result = MatchResult.Draw;
            if (match.Winner == player.Stone)
            {
                result = MatchResult.Win;
            }
            else if (match.Winner != Stone.None)
            {
                result = MatchResult.Lose;
            }

            User? user = await userDal.GetByIdAsync(player.UserId);
            if (user == null)
            {
                return;
            }

            // Điểm không xuống dưới 1000, lịch sử ghi số điểm thực tế thay đổi
            int delta = ScoreCalculator.GetScoreDelta(result, match.EndReason, moveCount);
            int newScore = ScoreCalculator.ApplyMinimum(user.Score + delta);
            int scoreChange = newScore - user.Score;

            user.Score = newScore;
            await userDal.UpdateAsync(user);

            await historyDal.AddAsync(new MatchHistory
            {
                UserId = player.UserId,
                OpponentId = opponent.UserId,
                Mode = GameMode.PvPOnline.ToString(),
                Result = result.ToString(),
                EndReason = match.EndReason.ToString(),
                BlockBothEndsRule = match.BlockBothEndsRule,
                MoveCount = moveCount,
                ScoreChange = scoreChange,
                PlayedAt = DateTime.Now
            });

            await notifier.SendAsync(player.ConnectionId, "GameOver", new GameOverDto
            {
                Result = result,
                EndReason = match.EndReason,
                ScoreChange = scoreChange,
                NewScore = newScore
            });

            // Người mất kết nối quá 30 giây: trận đã xử xong thì giải phóng phiên đăng nhập
            if (player.IsDisconnected)
            {
                SessionStore.RemoveByUserId(player.UserId);
            }
        }

        // ===================== Hàm phụ =====================

        private OnlineMatch? FindMatchOfConnection(string connectionId, out int userId)
        {
            userId = 0;
            OnlineUser? onlineUser = SessionStore.FindByConnection(connectionId);
            if (onlineUser == null)
            {
                return null;
            }

            userId = onlineUser.UserId;
            return FindMatchOfUser(userId);
        }

        private OnlineMatch? FindMatchOfUser(int userId)
        {
            lock (lockObject)
            {
                return matches.FirstOrDefault(m => m.HasPlayer(userId));
            }
        }

        private async Task SendAllAsync(List<Notification> notifications)
        {
            foreach (Notification notification in notifications)
            {
                await notifier.SendAsync(notification.ConnectionId, notification.Method, notification.Data);
            }
        }
    }
}
