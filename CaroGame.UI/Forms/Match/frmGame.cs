using CaroGame.BLL.Dtos;
using CaroGame.BLL.Game;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using CaroGame.BLL.Models;
using CaroGame.BLL.Settings;

namespace CaroGame.UI.Forms.Match
{
    // Màn trận đấu, dùng chung cho 3 chế độ.
    // - PvE, PvP Offline: gọi LocalMatch (tầng BLL) xử lý ngay trên máy.
    // - PvP Online: gửi thao tác lên server, server xử lý rồi gửi kết quả về, Form chỉ vẽ lại theo OnlineMatchState.
    public partial class frmGame : Form
    {
        // Tên các tin server gửi về trong trận Online (để gỡ đăng ký khi đóng Form)
        private static readonly string[] OnlineEventNames =
        {
            "GameStarted", "MoveMade", "MoveUndone", "ClockUpdate", "UndoRequested", "UndoAnswered",
            "UndoTimeout", "ChatMessage", "OpponentDisconnected", "OpponentReconnected", "GameOver", "MatchCanceled"
        };

        // Trận PvE/Offline (null nếu đang chơi Online)
        private readonly LocalMatch? localMatch;

        // Trận Online (null nếu đang chơi PvE/Offline)
        private readonly OnlineMatchState? onlineMatch;

        private readonly BotChatService? botChat;
        private HubConnection? hub;
        private frmUndoRequest? undoRequestForm;
        private bool surrenderedByMe;
        private int cellSize;

        // true nếu chọn "Tìm trận mới" hoặc đối thủ thoát sớm: frmModeSelect sẽ mở lại màn tìm trận
        public bool FindNewMatch { get; private set; }

        // PvE và PvP Offline. Người chơi 1 luôn cầm X, người chơi 2 (hoặc bot) luôn cầm O.
        public frmGame(GameMode mode, string player1Name)
        {
            InitializeComponent();

            localMatch = new LocalMatch(mode, player1Name);
            botChat = new BotChatService(ClientSettings.Load().Llm);
            Player1Timer.Tick += Player1Timer_Tick;
            AttachCommonEvents();
        }

        // PvP Online
        public frmGame(MatchFoundDto matchInfo)
        {
            InitializeComponent();

            onlineMatch = new OnlineMatchState(matchInfo);

            // Đồng hồ Online do server chạy và gửi về mỗi giây, client không tự đếm
            Player1Timer.Enabled = false;
            AttachCommonEvents();
        }

        // Gắn các sự kiện chưa được gắn trong Designer
        private void AttachCommonEvents()
        {
            pnlBoard.MouseClick += pnlBoard_MouseClick;
            btnSurrender.Click += btnSurrender_Click;
            FormClosing += frmGame_FormClosing;
        }

        private Board CurrentBoard
        {
            get { return localMatch != null ? localMatch.Board : onlineMatch!.Board; }
        }

        private async void frmGame_Load(object? sender, EventArgs e)
        {
            // Bàn cờ 20x20 ô vuông, kích thước ô tính theo kích thước Panel
            cellSize = Math.Min(pnlBoard.Width, pnlBoard.Height) / Board.Size;

            if (localMatch != null)
            {
                lblPlayer1Name.Text = localMatch.Player1Name + " (X)";
                lblPlayer2Name.Text = localMatch.Player2Name + " (O)";
                lblRule.Text = "Chặn 2 đầu: Bật";
                lblConnectionStatus.Text = "Offline";
            }
            else
            {
                lblPlayer1Name.Text = onlineMatch!.XName + " (X)" + (onlineMatch.MyStone == Stone.X ? " - Bạn" : "");
                lblPlayer2Name.Text = onlineMatch.OName + " (O)" + (onlineMatch.MyStone == Stone.O ? " - Bạn" : "");
                lblRule.Text = "Đang hỏi luật chặn 2 đầu...";
                lblConnectionStatus.Text = "Online";
                txtChatInput.MaxLength = 200;
                await ListenOnlineEventsAsync();
            }

            UpdateView();
        }

        // ===================== Bàn cờ =====================

        private async void pnlBoard_MouseClick(object? sender, MouseEventArgs e)
        {
            // Đổi tọa độ chuột (pixel) sang hàng, cột của bàn cờ
            int row = e.Y / cellSize;
            int col = e.X / cellSize;

            if (onlineMatch != null)
            {
                await SendOnlineMoveAsync(row, col);
            }
            else
            {
                await PlayLocalMoveAsync(row, col);
            }
        }

        private async Task PlayLocalMoveAsync(int row, int col)
        {
            // Ô ngoài bàn, ô đã có quân, trận đã kết thúc, hoặc đang tới lượt bot thì bỏ qua
            if (!localMatch!.PlayMove(row, col))
            {
                return;
            }

            SoundManager.PlayMoveSound();
            UpdateView();
            if (localMatch.IsOver)
            {
                await ShowLocalResultAsync();
                return;
            }

            // PvE: chờ 1 giây rồi bot mới đánh. Trong lúc chờ, giao diện vẫn hoạt động bình thường.
            if (localMatch.IsBotTurn)
            {
                await Task.Delay(1000);

                // Trong lúc chờ, người chơi có thể đã đóng màn trận hoặc đầu hàng
                if (IsDisposed || !localMatch.PlayBotMove())
                {
                    return;
                }

                SoundManager.PlayMoveSound();
                UpdateView();
                if (localMatch.IsOver)
                {
                    await ShowLocalResultAsync();
                }
            }
        }

        // Online: chỉ gửi lên server. Server kiểm tra nước đi, hợp lệ thì gửi "MoveMade" về cho cả 2 người.
        private async Task SendOnlineMoveAsync(int row, int col)
        {
            if (hub == null)
            {
                return;
            }

            try
            {
                await hub.InvokeAsync("PlayMove", row, col);
            }
            catch (Exception)
            {
                lblConnectionStatus.Text = "Không gửi được nước đi.";
            }
        }

        private void pnlBoard_Paint(object? sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawStones(e.Graphics);
        }

        private void DrawGrid(Graphics g)
        {
            int boardPixels = Board.Size * cellSize;
            for (int i = 0; i <= Board.Size; i++)
            {
                g.DrawLine(Pens.Gray, i * cellSize, 0, i * cellSize, boardPixels);
                g.DrawLine(Pens.Gray, 0, i * cellSize, boardPixels, i * cellSize);
            }
        }

        private void DrawStones(Graphics g)
        {
            Font font = new Font("Arial", cellSize / 2, FontStyle.Bold);

            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    Stone stone = CurrentBoard.GetStone(row, col);
                    int x = col * cellSize + cellSize / 5;
                    int y = row * cellSize + cellSize / 8;

                    if (stone == Stone.X)
                    {
                        g.DrawString("X", font, Brushes.Blue, x, y);
                    }
                    else if (stone == Stone.O)
                    {
                        g.DrawString("O", font, Brushes.Red, x, y);
                    }
                }
            }

            font.Dispose();
        }

        // ===================== Đồng hồ (PvE, Offline) =====================

        // Player1Timer chạy suốt trận PvE/Offline, mỗi giây gọi 1 lần (Interval = 1000 trong Designer).
        // Chỉ cập nhật chữ, KHÔNG vẽ lại bàn cờ, để bàn cờ không bị chớp mỗi giây.
        private async void Player1Timer_Tick(object? sender, EventArgs e)
        {
            bool timeUp = localMatch!.TickClock();
            UpdateLabels();

            if (timeUp)
            {
                await ShowLocalResultAsync();
            }
        }

        private string FormatTime(int seconds)
        {
            return (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
        }

        // ===================== Undo / FF =====================

        private void btnGameSettings_Click(object? sender, EventArgs e)
        {
            pnlGameSettings.Visible = !pnlGameSettings.Visible;
        }

        private async void btnUndo_Click(object? sender, EventArgs e)
        {
            pnlGameSettings.Visible = false;

            if (localMatch != null)
            {
                if (localMatch.Undo())
                {
                    UpdateView();
                }
                return;
            }

            // Online: gửi lời xin lên server, server trả về lý do nếu không xin được
            if (hub == null)
            {
                return;
            }

            try
            {
                string error = await hub.InvokeAsync<string>("RequestUndo");
                if (error != "")
                {
                    MessageBox.Show(error, "Undo");
                }
            }
            catch (Exception)
            {
                lblConnectionStatus.Text = "Không kết nối được server.";
            }
        }

        private async void btnSurrender_Click(object? sender, EventArgs e)
        {
            pnlGameSettings.Visible = false;

            if (localMatch != null)
            {
                localMatch.Surrender();
                UpdateView();
                await ShowLocalResultAsync();
                return;
            }

            // Online: server xử thua rồi gửi "GameOver" (hoặc "MatchCanceled" nếu chưa có nước đi nào)
            surrenderedByMe = true;
            await SurrenderOnlineAsync();
        }

        private async Task SurrenderOnlineAsync()
        {
            if (hub == null)
            {
                return;
            }

            try
            {
                await hub.InvokeAsync("Surrender");
            }
            catch (Exception)
            {
                // Mất kết nối thì server tự xử thua sau 30 giây
            }
        }

        // Không có nút thoát riêng: đóng trận khi chưa kết thúc chính là đầu hàng
        private async void frmGame_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (localMatch != null)
            {
                if (!localMatch.IsOver)
                {
                    localMatch.Surrender();
                    await SaveLocalResultAsync();
                }
                return;
            }

            StopListeningOnlineEvents();
            if (!onlineMatch!.IsOver)
            {
                await SurrenderOnlineAsync();
            }
        }

        // ===================== Kết thúc trận =====================

        private async Task ShowLocalResultAsync()
        {
            await SaveLocalResultAsync();

            string resultText;
            if (localMatch!.Winner == Stone.None)
            {
                resultText = "Hòa";
            }
            else if (localMatch.Mode == GameMode.PvE)
            {
                resultText = localMatch.Winner == LocalMatch.Player1Stone ? "Bạn thắng!" : "Bạn thua!";
            }
            else
            {
                resultText = localMatch.GetPlayerName(localMatch.Winner) + " thắng!";
            }

            ShowResult(resultText, GameTexts.GetModeText(localMatch.Mode), GameTexts.GetReasonText(localMatch.EndReason), "", "Chơi lại");
        }

        // Đã đăng nhập thì gửi kết quả PvE/Offline lên server để lưu lịch sử. Mất kết nối thì bỏ qua.
        private async Task SaveLocalResultAsync()
        {
            if (!Session.IsLoggedIn || localMatch == null)
            {
                return;
            }

            try
            {
                HubConnection connection = await ServerConnection.ConnectAsync();
                await connection.InvokeAsync("SaveLocalMatch", localMatch.CreateResultForPlayer1());
            }
            catch (Exception)
            {
                // Bỏ qua, không lưu
            }
        }

        // Mở hộp thoại kết quả. "Chơi lại" (PvE/Offline) bắt đầu ván mới, "Tìm trận mới" (Online) quay về màn tìm trận.
        private void ShowResult(string resultText, string modeText, string reasonText, string scoreText, string playAgainText)
        {
            frmResult resultForm = new frmResult(resultText, modeText, reasonText, scoreText, playAgainText);
            DialogResult choice = resultForm.ShowDialog(this);

            if (choice == DialogResult.Retry && localMatch != null)
            {
                localMatch.Start();
                UpdateView();
                return;
            }

            if (choice == DialogResult.Retry)
            {
                FindNewMatch = true;
            }
            Close();
        }

        // ===================== Chat =====================

        private async void btnSendChat_Click(object? sender, EventArgs e)
        {
            string message = txtChatInput.Text.Trim();
            if (message == "")
            {
                return;
            }

            txtChatInput.Clear();

            // Online: tin nhắn đi qua server, server gửi lại cho cả 2 người (kể cả người gửi)
            if (onlineMatch != null)
            {
                if (hub != null)
                {
                    try
                    {
                        await hub.InvokeAsync("SendChat", message);
                    }
                    catch (Exception)
                    {
                        lblConnectionStatus.Text = "Không gửi được tin nhắn.";
                    }
                }
                return;
            }

            // PvP Offline: tin nhắn chỉ hiển thị, không có phản hồi
            if (localMatch!.Mode == GameMode.PvPOffline)
            {
                listBoxChat.Items.Add(message);
                return;
            }

            listBoxChat.Items.Add(localMatch.Player1Name + ": " + message);

            // await chạy tiếp trên luồng giao diện nên cập nhật listBoxChat trực tiếp được
            string reply = await botChat!.GetReplyAsync(message, localMatch);

            // Trong lúc chờ bot trả lời, người chơi có thể đã đóng màn trận
            if (IsDisposed)
            {
                return;
            }

            listBoxChat.Items.Add("Bot: " + reply);
        }

        // ===================== Tin từ server (Online) =====================

        // Tin từ server đến trên luồng khác, không phải luồng giao diện.
        // Control chỉ được sửa trên luồng giao diện, nên mọi xử lý đều đưa qua BeginInvoke.
        // Dùng BeginInvoke (không chờ) thay vì Invoke để không chặn luồng nhận tin của SignalR:
        // ví dụ đang mở hộp thoại Undo vẫn nhận được tin "UndoTimeout" để tự đóng hộp thoại.
        private async Task ListenOnlineEventsAsync()
        {
            try
            {
                hub = await ServerConnection.ConnectAsync();
            }
            catch (Exception)
            {
                lblConnectionStatus.Text = "Không kết nối được server.";
                return;
            }

            hub.On<GameStartedDto>("GameStarted", info => BeginInvoke(new Action(() => OnGameStarted(info))));
            hub.On<MoveDto>("MoveMade", move => BeginInvoke(new Action(() => OnMoveMade(move))));
            hub.On<MoveDto>("MoveUndone", move => BeginInvoke(new Action(() => OnMoveUndone(move))));
            hub.On<ClockDto>("ClockUpdate", clock => BeginInvoke(new Action(() => OnClockUpdate(clock))));
            hub.On<string>("UndoRequested", name => BeginInvoke(new Action(() => OnUndoRequested(name))));
            hub.On<UndoAnswerDto>("UndoAnswered", answer => BeginInvoke(new Action(() => OnUndoAnswered(answer))));
            hub.On<string>("UndoTimeout", text => BeginInvoke(new Action(() => CloseUndoRequest())));
            hub.On<ChatDto>("ChatMessage", chat => BeginInvoke(new Action(() => OnChatMessage(chat))));
            hub.On<string>("OpponentDisconnected", text => BeginInvoke(new Action(() => ShowConnectionStatus("Đối thủ mất kết nối, chờ tối đa 30 giây..."))));
            hub.On<string>("OpponentReconnected", text => BeginInvoke(new Action(() => ShowConnectionStatus("Online"))));
            hub.On<GameOverDto>("GameOver", result => BeginInvoke(new Action(() => OnGameOver(result))));
            hub.On<string>("MatchCanceled", message => BeginInvoke(new Action(() => OnMatchCanceled(message))));

            hub.Reconnecting += Hub_Reconnecting;
            hub.Reconnected += Hub_Reconnected;
        }

        private void StopListeningOnlineEvents()
        {
            if (hub == null)
            {
                return;
            }

            foreach (string eventName in OnlineEventNames)
            {
                hub.Remove(eventName);
            }
            hub.Reconnecting -= Hub_Reconnecting;
            hub.Reconnected -= Hub_Reconnected;
        }

        private Task Hub_Reconnecting(Exception? error)
        {
            BeginInvoke(new Action(() => ShowConnectionStatus("Mất kết nối, đang kết nối lại...")));
            return Task.CompletedTask;
        }

        private Task Hub_Reconnected(string? newConnectionId)
        {
            BeginInvoke(new Action(() => ShowConnectionStatus("Online")));
            return Task.CompletedTask;
        }

        private void ShowConnectionStatus(string text)
        {
            lblConnectionStatus.Text = text;
        }

        private void OnGameStarted(GameStartedDto info)
        {
            onlineMatch!.Start(info);
            lblRule.Text = info.BlockBothEndsRule ? "Chặn 2 đầu: Bật" : "Chặn 2 đầu: Tắt";
            UpdateView();
        }

        private void OnMoveMade(MoveDto move)
        {
            onlineMatch!.ApplyMove(move);
            SoundManager.PlayMoveSound();
            UpdateView();
        }

        private void OnMoveUndone(MoveDto move)
        {
            onlineMatch!.ApplyUndo(move);
            UpdateView();
        }

        private void OnClockUpdate(ClockDto clock)
        {
            onlineMatch!.ApplyClock(clock);
            UpdateLabels();
        }

        // Đối thủ xin Undo: hỏi người chơi. Hết 15 giây server gửi "UndoTimeout" để tự đóng hộp thoại.
        private async void OnUndoRequested(string requesterName)
        {
            undoRequestForm = new frmUndoRequest();
            DialogResult answer = undoRequestForm.ShowDialog(this);
            undoRequestForm = null;

            // Bị đóng vì hết giờ (Cancel) thì server đã tự tính là từ chối, không gửi gì nữa
            if (answer != DialogResult.Yes && answer != DialogResult.No)
            {
                return;
            }

            try
            {
                await hub!.InvokeAsync("AnswerUndo", answer == DialogResult.Yes);
            }
            catch (Exception)
            {
                lblConnectionStatus.Text = "Không kết nối được server.";
            }
        }

        private void CloseUndoRequest()
        {
            if (undoRequestForm != null)
            {
                undoRequestForm.Close();
            }
        }

        private void OnUndoAnswered(UndoAnswerDto answer)
        {
            // Được đồng ý thì bàn cờ đã tự lùi nhờ tin "MoveUndone", chỉ báo khi bị từ chối
            if (!answer.Accepted)
            {
                MessageBox.Show(answer.Message, "Undo");
            }
        }

        private void OnChatMessage(ChatDto chat)
        {
            listBoxChat.Items.Add(chat.Sender + ": " + chat.Text);
        }

        private void OnGameOver(GameOverDto result)
        {
            onlineMatch!.Finish();
            CloseUndoRequest();
            Session.UpdateScore(result.NewScore);
            UpdateView();

            string resultText = "Hòa";
            if (result.Result == MatchResult.Win)
            {
                resultText = "Bạn thắng!";
            }
            else if (result.Result == MatchResult.Lose)
            {
                resultText = "Bạn thua!";
            }

            string sign = result.ScoreChange > 0 ? "+" : "";
            string scoreText = "Điểm thay đổi: " + sign + result.ScoreChange + " (điểm mới: " + result.NewScore + ")";

            ShowResult(resultText, GameTexts.GetModeText(GameMode.PvPOnline), GameTexts.GetReasonText(result.EndReason), scoreText, "Tìm trận mới");
        }

        // Trận bị hủy (có người thoát trước nước đi đầu tiên): không ai đổi điểm.
        // Người còn lại quay về màn tìm trận, người bấm thoát thì về menu.
        private void OnMatchCanceled(string message)
        {
            onlineMatch!.Finish();
            CloseUndoRequest();
            MessageBox.Show(message, "Trận bị hủy");
            FindNewMatch = !surrenderedByMe;
            Close();
        }

        // ===================== Cập nhật giao diện =====================

        // Vẽ lại bàn cờ và cập nhật chữ. Chỉ gọi khi bàn cờ thay đổi (đánh, Undo, chơi lại).
        private void UpdateView()
        {
            pnlBoard.Invalidate();
            UpdateLabels();
        }

        // Cập nhật đồng hồ và lượt đi
        private void UpdateLabels()
        {
            if (localMatch != null)
            {
                lblPlayer1Timer.Text = FormatTime(localMatch.Clock.SecondsLeftX);
                lblPlayer2Timer.Text = FormatTime(localMatch.Clock.SecondsLeftO);
                lblTurn.Text = localMatch.IsOver ? "Hết trận" : "Lượt: " + localMatch.GetPlayerName(localMatch.CurrentTurn);
                return;
            }

            lblPlayer1Timer.Text = FormatTime(onlineMatch!.SecondsLeftX);
            lblPlayer2Timer.Text = FormatTime(onlineMatch.SecondsLeftO);

            if (onlineMatch.IsOver)
            {
                lblTurn.Text = "Hết trận";
            }
            else if (!onlineMatch.IsStarted)
            {
                lblTurn.Text = "Chờ chọn luật...";
            }
            else
            {
                lblTurn.Text = "Lượt: " + onlineMatch.GetPlayerName(onlineMatch.CurrentTurn);
            }
        }
    }
}
