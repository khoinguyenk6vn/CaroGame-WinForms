using CaroGame.Shared.Game;
using CaroGame_WinForms.Game;
using CaroGame_WinForms.Services;

namespace CaroGame_WinForms
{
    // Màn trận đấu cho PvE và PvP Offline.
    // Form chỉ nhận thao tác của người chơi, gọi LocalMatch xử lý, rồi vẽ lại theo trạng thái trận.
    public partial class frmGame : Form
    {
        private readonly LocalMatch match;
        private readonly string player1Name;
        private readonly string player2Name;
        private readonly BotChatService botChat;
        private int cellSize;

        // Người chơi 1 luôn cầm X, người chơi 2 (hoặc bot) luôn cầm O
        public frmGame(GameMode mode, string player1Name)
        {
            InitializeComponent();

            match = new LocalMatch(mode);
            this.player1Name = player1Name;
            player2Name = mode == GameMode.PvE ? "Bot" : "Người chơi 2";
            botChat = new BotChatService(ClientSettings.Load().Llm);

            // Gắn các sự kiện chưa được gắn trong Designer
            pnlBoard.MouseClick += pnlBoard_MouseClick;
            btnSurrender.Click += btnSurrender_Click;
            Player1Timer.Tick += Player1Timer_Tick;
            FormClosing += frmGame_FormClosing;
        }

        private void frmGame_Load(object? sender, EventArgs e)
        {
            // Bàn cờ 20x20 ô vuông, kích thước ô tính theo kích thước Panel
            cellSize = Math.Min(pnlBoard.Width, pnlBoard.Height) / Board.Size;

            lblPlayer1Name.Text = player1Name + " (X)";
            lblPlayer2Name.Text = player2Name + " (O)";
            lblRule.Text = "Chặn 2 đầu: Bật";
            lblConnectionStatus.Text = "Offline";

            UpdateView();
        }

        // ===================== Bàn cờ =====================

        private async void pnlBoard_MouseClick(object? sender, MouseEventArgs e)
        {
            // Đổi tọa độ chuột (pixel) sang hàng, cột của bàn cờ
            int row = e.Y / cellSize;
            int col = e.X / cellSize;

            // Ô ngoài bàn, ô đã có quân, trận đã kết thúc, hoặc đang tới lượt bot thì bỏ qua
            if (!match.PlayMove(row, col))
            {
                return;
            }

            UpdateView();
            if (match.IsOver)
            {
                ShowResult();
                return;
            }

            // PvE: chờ 1 giây rồi bot mới đánh. Trong lúc chờ, giao diện vẫn hoạt động bình thường.
            if (match.IsBotTurn)
            {
                await Task.Delay(1000);

                // Trong lúc chờ, người chơi có thể đã đóng màn trận hoặc đầu hàng
                if (IsDisposed || !match.PlayBotMove())
                {
                    return;
                }

                UpdateView();
                if (match.IsOver)
                {
                    ShowResult();
                }
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
                    Stone stone = match.Board.GetStone(row, col);
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

        // ===================== Đồng hồ =====================

        // Player1Timer chạy suốt trận, mỗi giây gọi 1 lần (Interval = 1000 trong Designer).
        // Chỉ cập nhật chữ, KHÔNG vẽ lại bàn cờ, để bàn cờ không bị chớp mỗi giây.
        private void Player1Timer_Tick(object? sender, EventArgs e)
        {
            bool timeUp = match.TickClock();
            UpdateLabels();

            if (timeUp)
            {
                ShowResult();
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

        private void btnUndo_Click(object? sender, EventArgs e)
        {
            pnlGameSettings.Visible = false;

            if (match.Undo())
            {
                UpdateView();
            }
        }

        private void btnSurrender_Click(object? sender, EventArgs e)
        {
            pnlGameSettings.Visible = false;

            match.Surrender();
            UpdateView();
            ShowResult();
        }

        // Không có nút thoát riêng: đóng trận khi chưa kết thúc chính là đầu hàng
        private void frmGame_FormClosing(object? sender, FormClosingEventArgs e)
        {
            match.Surrender();
        }

        // ===================== Kết thúc trận =====================

        private void ShowResult()
        {
            frmResult resultForm = new frmResult(GetResultText(), GetModeText(), GetReasonText());
            DialogResult choice = resultForm.ShowDialog(this);

            if (choice == DialogResult.Retry)
            {
                match.Start();
                UpdateView();
            }
            else
            {
                Close();
            }
        }

        private string GetResultText()
        {
            if (match.Winner == Stone.None)
            {
                return "Hòa";
            }

            if (match.Mode == GameMode.PvE)
            {
                return match.Winner == LocalMatch.Player1Stone ? "Bạn thắng!" : "Bạn thua!";
            }

            return GetPlayerName(match.Winner) + " thắng!";
        }

        private string GetModeText()
        {
            return match.Mode == GameMode.PvE ? "PvE" : "PvP Offline";
        }

        private string GetReasonText()
        {
            switch (match.EndReason)
            {
                case EndReason.FiveInRow:
                    return "Đủ 5 quân";
                case EndReason.Timeout:
                    return "Hết giờ";
                case EndReason.Forfeit:
                    return "Đầu hàng";
                default:
                    return "Bàn cờ đầy";
            }
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

            // PvP Offline: tin nhắn chỉ hiển thị, không có phản hồi
            if (match.Mode == GameMode.PvPOffline)
            {
                listBoxChat.Items.Add(message);
                return;
            }

            listBoxChat.Items.Add(player1Name + ": " + message);

            // await chạy tiếp trên luồng giao diện nên cập nhật listBoxChat trực tiếp được
            string reply = await botChat.GetReplyAsync(message, DescribeMatchSituation());

            // Trong lúc chờ bot trả lời, người chơi có thể đã đóng màn trận
            if (IsDisposed)
            {
                return;
            }

            listBoxChat.Items.Add("Bot: " + reply);
        }

        // Tình hình trận gửi kèm cho LLM, không chứa thông tin cá nhân của người chơi
        private string DescribeMatchSituation()
        {
            return "Bàn cờ 20x20, đã đánh " + match.Board.MoveCount + " nước. Người chơi cầm X, bot cầm O.";
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
            lblPlayer1Timer.Text = FormatTime(match.Clock.SecondsLeftX);
            lblPlayer2Timer.Text = FormatTime(match.Clock.SecondsLeftO);

            if (match.IsOver)
            {
                lblTurn.Text = "Hết trận";
            }
            else
            {
                lblTurn.Text = "Lượt: " + GetPlayerName(match.CurrentTurn);
            }
        }

        private string GetPlayerName(Stone stone)
        {
            return stone == LocalMatch.Player1Stone ? player1Name : player2Name;
        }
    }
}
