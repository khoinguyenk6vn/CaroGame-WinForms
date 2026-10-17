using CaroGame.BLL.Dtos;
using CaroGame.BLL.Game;

namespace CaroGame.BLL.Online
{
    // Một trận PvP Online trên server. Server là nơi quyết định duy nhất: nước đi, đồng hồ, Undo, kết quả.
    // Mọi hàm public đều nằm trong lock, nên các sự kiện của trận (nước đi, hết giờ, xin Undo, mất kết nối...)
    // được xử lý lần lượt từng cái, không bị chồng lên nhau.
    // Các tin cần gửi cho client được gom vào outbox, OnlineGameManager lấy ra bằng TakeNotifications() rồi gửi.
    public class OnlineMatch
    {
        private const int RuleVoteSeconds = 15;
        private const int UndoAnswerSeconds = 15;
        private const int ReconnectSeconds = 30;
        private const int MaxUndoRejections = 3;
        private const int MaxChatLength = 200;

        private readonly object lockObject = new object();
        private readonly List<Notification> outbox = new List<Notification>();

        public OnlinePlayer PlayerX { get; }
        public OnlinePlayer PlayerO { get; }
        public Board Board { get; } = new Board();
        public GameClock Clock { get; } = new GameClock();
        public MatchPhase Phase { get; private set; } = MatchPhase.VotingRule;
        public bool BlockBothEndsRule { get; private set; }
        public Stone CurrentTurn { get; private set; } = Stone.X;
        public bool IsCanceled { get; private set; }

        // Stone.None nghĩa là hòa
        public Stone Winner { get; private set; } = Stone.None;
        public EndReason EndReason { get; private set; }

        private int voteSecondsLeft = RuleVoteSeconds;

        // Người đang xin Undo (null nếu không có ai xin)
        private OnlinePlayer? undoRequester;
        private int undoSecondsLeft;

        // Người đang mất kết nối (null nếu cả 2 đều đang kết nối)
        private OnlinePlayer? disconnectedPlayer;
        private int reconnectSecondsLeft;

        public OnlineMatch(OnlinePlayer playerX, OnlinePlayer playerO)
        {
            PlayerX = playerX;
            PlayerO = playerO;
            PlayerX.Stone = Stone.X;
            PlayerO.Stone = Stone.O;

            // Báo cho 2 người là đã ghép được trận, kèm quân của mỗi người
            AddMessage(PlayerX, "MatchFound", new MatchFoundDto { XName = PlayerX.Username, OName = PlayerO.Username, MyStone = Stone.X });
            AddMessage(PlayerO, "MatchFound", new MatchFoundDto { XName = PlayerX.Username, OName = PlayerO.Username, MyStone = Stone.O });
        }

        public bool HasPlayer(int userId)
        {
            return PlayerX.UserId == userId || PlayerO.UserId == userId;
        }

        // Lấy ra các tin đang chờ gửi
        public List<Notification> TakeNotifications()
        {
            lock (lockObject)
            {
                var notifications = new List<Notification>(outbox);
                outbox.Clear();
                return notifications;
            }
        }

        // ===================== Hỏi luật chặn 2 đầu =====================

        public void VoteRule(int userId, bool agree)
        {
            lock (lockObject)
            {
                if (Phase != MatchPhase.VotingRule)
                {
                    return;
                }

                GetPlayer(userId).AgreesBlockRule = agree;

                if (PlayerX.AgreesBlockRule != null && PlayerO.AgreesBlockRule != null)
                {
                    StartGame();
                }
            }
        }

        // Chỉ khi cả 2 đồng ý mới áp dụng luật chặn 2 đầu. Không trả lời tính là không đồng ý.
        private void StartGame()
        {
            BlockBothEndsRule = PlayerX.AgreesBlockRule == true && PlayerO.AgreesBlockRule == true;
            Phase = MatchPhase.Playing;
            Clock.StartFor(Stone.X);
            AddMessageToBoth("GameStarted", new GameStartedDto { BlockBothEndsRule = BlockBothEndsRule });
        }

        // ===================== Nước đi =====================

        public void PlayMove(int userId, int row, int col)
        {
            lock (lockObject)
            {
                OnlinePlayer player = GetPlayer(userId);

                // Bỏ qua nếu chưa bắt đầu, đang tạm dừng, không phải lượt người này, hoặc ô không hợp lệ
                if (Phase != MatchPhase.Playing || IsPaused() || player.Stone != CurrentTurn
                    || !GameRules.IsValidMove(Board, row, col))
                {
                    return;
                }

                Board.PlaceStone(row, col, player.Stone);

                if (GameRules.IsWinningMove(Board, row, col, BlockBothEndsRule))
                {
                    Finish(player.Stone, EndReason.FiveInRow);
                }
                else if (Board.IsFull)
                {
                    Finish(Stone.None, EndReason.Draw);
                }
                else
                {
                    CurrentTurn = GameRules.Opponent(CurrentTurn);
                    Clock.StartFor(CurrentTurn);
                }

                AddMessageToBoth("MoveMade", new MoveDto { Row = row, Col = col, Stone = player.Stone, NextTurn = CurrentTurn });
            }
        }

        // Đang chờ trả lời Undo hoặc có người mất kết nối thì trận tạm dừng
        private bool IsPaused()
        {
            return undoRequester != null || disconnectedPlayer != null;
        }

        // ===================== Undo =====================

        // Trả về "" nếu đã gửi lời xin, ngược lại trả về lý do không xin được
        public string RequestUndo(int userId)
        {
            lock (lockObject)
            {
                OnlinePlayer player = GetPlayer(userId);

                if (Phase != MatchPhase.Playing || IsPaused())
                {
                    return "Không thể xin đi lại lúc này.";
                }

                // Chỉ được xin ngay sau nước của mình, trước khi đối thủ đi tiếp
                Move? lastMove = Board.LastMove;
                if (lastMove == null || lastMove.Stone != player.Stone)
                {
                    return "Chỉ được xin đi lại ngay sau nước của mình.";
                }

                if (player.UndoRejections >= MaxUndoRejections)
                {
                    return "Bạn đã bị từ chối 3 lần, không được xin đi lại nữa trong trận này.";
                }

                // Đồng hồ cả 2 dừng trong lúc chờ trả lời
                undoRequester = player;
                undoSecondsLeft = UndoAnswerSeconds;
                Clock.Stop();
                AddMessage(GetOpponent(player), "UndoRequested", player.Username);
                return "";
            }
        }

        public void AnswerUndo(int userId, bool accept)
        {
            lock (lockObject)
            {
                // Chỉ đối thủ của người xin mới được trả lời
                if (undoRequester == null || undoRequester.UserId == userId)
                {
                    return;
                }

                if (accept)
                {
                    AcceptUndo();
                }
                else
                {
                    DeclineUndo("Đối thủ đã từ chối");
                }
            }
        }

        // Lùi 1 nước, trả lượt cho người xin. Không hoàn lại thời gian đã dùng.
        private void AcceptUndo()
        {
            OnlinePlayer requester = undoRequester!;
            undoRequester = null;

            Move removedMove = Board.UndoLastMove()!;
            CurrentTurn = requester.Stone;
            Clock.StartFor(CurrentTurn);

            AddMessageToBoth("MoveUndone", new MoveDto { Row = removedMove.Row, Col = removedMove.Col, Stone = removedMove.Stone, NextTurn = CurrentTurn });
            AddMessage(requester, "UndoAnswered", new UndoAnswerDto { Accepted = true, Message = "Đối thủ đã đồng ý cho đi lại." });
        }

        // Từ chối hoặc hết 15 giây không trả lời: cộng 1 lần bị từ chối, đồng hồ chạy lại
        private void DeclineUndo(string reason)
        {
            OnlinePlayer requester = undoRequester!;
            undoRequester = null;

            requester.UndoRejections++;
            Clock.StartFor(CurrentTurn);

            AddMessage(requester, "UndoAnswered", new UndoAnswerDto
            {
                Accepted = false,
                Message = reason + " (bị từ chối " + requester.UndoRejections + "/3 lần)."
            });
        }

        // ===================== Chat =====================

        public void SendChat(int userId, string text)
        {
            lock (lockObject)
            {
                text = text.Trim();
                if (text == "" || text.Length > MaxChatLength)
                {
                    return;
                }

                AddMessageToBoth("ChatMessage", new ChatDto { Sender = GetPlayer(userId).Username, Text = text });
            }
        }

        // ===================== Đầu hàng, mất kết nối =====================

        // FF hoặc thoát trận
        public void Surrender(int userId)
        {
            lock (lockObject)
            {
                if (Phase == MatchPhase.Finished)
                {
                    return;
                }

                OnlinePlayer player = GetPlayer(userId);

                if (Phase == MatchPhase.VotingRule || Board.MoveCount == 0)
                {
                    Cancel(player);
                    return;
                }

                Finish(GetOpponent(player).Stone, EndReason.Forfeit);
            }
        }

        // Trả về true nếu trận đang chờ người này kết nối lại (server phải giữ phiên đăng nhập của họ)
        public bool Disconnect(int userId)
        {
            lock (lockObject)
            {
                if (Phase == MatchPhase.Finished)
                {
                    return false;
                }

                OnlinePlayer player = GetPlayer(userId);

                if (Phase == MatchPhase.VotingRule || Board.MoveCount == 0)
                {
                    Cancel(player);
                    return false;
                }

                player.IsDisconnected = true;

                // Người mất kết nối trước bị tính giờ trước. Nếu người kia cũng mất kết nối thì chờ tới lượt.
                if (disconnectedPlayer == null)
                {
                    disconnectedPlayer = player;
                    reconnectSecondsLeft = ReconnectSeconds;
                }

                // Có 30 giây để kết nối lại, đồng hồ cả 2 tạm dừng
                Clock.Stop();
                AddMessage(GetOpponent(player), "OpponentDisconnected", "");
                return true;
            }
        }

        public void Reconnect(int userId, string newConnectionId)
        {
            lock (lockObject)
            {
                OnlinePlayer player = GetPlayer(userId);
                player.ConnectionId = newConnectionId;

                if (!player.IsDisconnected)
                {
                    return;
                }

                player.IsDisconnected = false;

                if (disconnectedPlayer == player)
                {
                    disconnectedPlayer = null;

                    // Nếu đối thủ cũng đang mất kết nối thì bắt đầu đếm 30 giây cho đối thủ
                    OnlinePlayer opponent = GetOpponent(player);
                    if (opponent.IsDisconnected)
                    {
                        disconnectedPlayer = opponent;
                        reconnectSecondsLeft = ReconnectSeconds;
                    }
                }

                if (Phase == MatchPhase.Playing && !IsPaused())
                {
                    Clock.StartFor(CurrentTurn);
                }

                AddMessage(GetOpponent(player), "OpponentReconnected", "");
            }
        }

        // Đối thủ thoát lúc đang hỏi luật hoặc trước nước đi đầu tiên: hủy trận, không ai đổi điểm
        private void Cancel(OnlinePlayer leaver)
        {
            Phase = MatchPhase.Finished;
            IsCanceled = true;
            Clock.Stop();
            AddMessage(GetOpponent(leaver), "MatchCanceled", "Đối thủ đã thoát, trận bị hủy.");
            AddMessage(leaver, "MatchCanceled", "Trận bị hủy vì chưa có nước đi nào.");
        }

        // ===================== Mỗi giây =====================

        // OnlineGameManager gọi hàm này mỗi giây một lần
        public void Tick()
        {
            lock (lockObject)
            {
                if (Phase == MatchPhase.VotingRule)
                {
                    voteSecondsLeft--;
                    if (voteSecondsLeft <= 0)
                    {
                        StartGame();
                    }
                    return;
                }

                if (Phase != MatchPhase.Playing)
                {
                    return;
                }

                // Mất kết nối quá 30 giây: xử thua như FF
                if (disconnectedPlayer != null)
                {
                    reconnectSecondsLeft--;
                    if (reconnectSecondsLeft <= 0)
                    {
                        Finish(GetOpponent(disconnectedPlayer).Stone, EndReason.Disconnect);
                    }
                    return;
                }

                // Hết 15 giây không trả lời Undo: tính là từ chối, đóng hộp thoại bên người được hỏi
                if (undoRequester != null)
                {
                    undoSecondsLeft--;
                    if (undoSecondsLeft <= 0)
                    {
                        AddMessage(GetOpponent(undoRequester), "UndoTimeout", "");
                        DeclineUndo("Đối thủ không trả lời");
                    }
                    return;
                }

                Clock.TickOneSecond();
                AddMessageToBoth("ClockUpdate", new ClockDto { SecondsLeftX = Clock.SecondsLeftX, SecondsLeftO = Clock.SecondsLeftO });

                if (Clock.IsTimeUp(CurrentTurn))
                {
                    Finish(GameRules.Opponent(CurrentTurn), EndReason.Timeout);
                }
            }
        }

        // ===================== Hàm phụ =====================

        private void Finish(Stone winner, EndReason reason)
        {
            Phase = MatchPhase.Finished;
            Winner = winner;
            EndReason = reason;
            Clock.Stop();
            undoRequester = null;
        }

        private OnlinePlayer GetPlayer(int userId)
        {
            return PlayerX.UserId == userId ? PlayerX : PlayerO;
        }

        private OnlinePlayer GetOpponent(OnlinePlayer player)
        {
            return player == PlayerX ? PlayerO : PlayerX;
        }

        private void AddMessage(OnlinePlayer player, string method, object data)
        {
            outbox.Add(new Notification(player.ConnectionId, method, data));
        }

        private void AddMessageToBoth(string method, object data)
        {
            AddMessage(PlayerX, method, data);
            AddMessage(PlayerO, method, data);
        }
    }
}
