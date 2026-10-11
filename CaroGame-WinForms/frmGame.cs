using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CaroGame_WinForms
{
    public partial class frmGame : Form
    {
        private const int BoardSize = 20; // Bàn cờ 20x20
        private int cellSize;             // Kích thước mỗi ô tính động theo kích thước Panel
        private int[,] boardData = new int[BoardSize, BoardSize]; // 0: trống, 1: Player 1, 2: Player 2
        private bool myTurn = true;       // Kiểm tra có phải lượt của mình không

        public frmGame()
        {
            InitializeComponent();

            // Đăng ký sự kiện vẽ và click cho bàn cờ (Ngoại lệ được phép vẽ bằng code)
            pnlBoard.Paint += pnlBoard_Paint;
            pnlBoard.MouseClick += pnlBoard_MouseClick;

            // Bật DoubleBuffered để vẽ bàn cờ mượt không bị nháy (flicker)
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, pnlBoard, new object[] { true });
        }
        private void frmGame_Load(object sender, EventArgs e)
        {
            // Tính toán kích thước mỗi ô dựa trên chiều rộng hiện tại của Panel
            cellSize = pnlBoard.Width / BoardSize;
            pnlGameSettings.Visible = false; // Ẩn menu cài đặt lúc đầu
            lblConnectionStatus.Text = "Trạng thái: Đã kết nối";
        }

        // 1. Xử lý sự kiện bấm nút mở menu cài đặt (Undo / FF)
        private void btnGameSettings_Click(object sender, EventArgs e)
        {
            pnlGameSettings.Visible = !pnlGameSettings.Visible;
        }

        // 2. Vẽ lưới bàn cờ và các quân cờ đã đánh
        private void pnlBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = Pens.Gray;
            Brush brushX = Brushes.Blue;
            Brush brushO = Brushes.Red;
            Font font = new Font("Arial", cellSize * 0.6f, FontStyle.Bold);
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Vẽ các đường kẻ bàn cờ 20x20
            for (int i = 0; i <= BoardSize; i++)
            {
                g.DrawLine(pen, i * cellSize, 0, i * cellSize, BoardSize * cellSize);
                g.DrawLine(pen, 0, i * cellSize, BoardSize * cellSize, i * cellSize);
            }

            // Vẽ các quân cờ đã đánh lên bàn cờ
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    Rectangle rect = new Rectangle(c * cellSize, r * cellSize, cellSize, cellSize);
                    if (boardData[r, c] == 1)
                    {
                        g.DrawString("X", font, brushX, rect, sf);
                    }
                    else if (boardData[r, c] == 2)
                    {
                        g.DrawString("O", font, brushO, rect, sf);
                    }
                }
            }
        }

        // 3. Xử lý khi người chơi click chuột lên bàn cờ để đánh quân
        private void pnlBoard_MouseClick(object sender, MouseEventArgs e)
        {
            if (!myTurn) return; // Không phải lượt của mình thì bỏ qua

            int c = e.X / cellSize;
            int r = e.Y / cellSize;

            // Kiểm tra ô hợp lệ và chưa có ai đánh
            if (r >= 0 && r < BoardSize && c >= 0 && c < BoardSize && boardData[r, c] == 0)
            {
                boardData[r, c] = 1; // Giả lập đánh quân X
                myTurn = false;      // Chuyển lượt
                lblTurn.Text = "Lượt của đối thủ";

                pnlBoard.Invalidate(); // Yêu cầu vẽ lại bàn cờ ngay lập tức

                // TODO: Gửi nước đi (r, c) này lên Server qua SignalR nếu là chế độ Online
            }
        }

        // 4. Xử lý sự kiện gửi tin nhắn chat
        private void btnSendChat_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtChatInput.Text))
            {
                listBoxChat.Items.Add("Tôi: " + txtChatInput.Text);
                txtChatInput.Clear();
                txtChatInput.Focus();
            }
        }

        // 5. Ví dụ hàm cập nhật giao diện an toàn từ luồng khác (SignalR Thread Safe)
        public void UpdateTimerFromServer(string player1Time, string player2Time)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateTimerFromServer(player1Time, player2Time)));
                return;
            }
            lblPlayer1Timer.Text = player1Time;
            lblPlayer2Timer.Text = player2Time;
        }


    }
}
