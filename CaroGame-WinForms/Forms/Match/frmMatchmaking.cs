using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CaroGame_WinForms
{
    using Timer = System.Windows.Forms.Timer;
    public partial class frmMatchmaking : Form
    {
        private int waitingSeconds = 0;
        private Timer searchTimer;

        public frmMatchmaking()
        {
            InitializeComponent();
        }

        private void frmMatchmaking_Load(object sender, EventArgs e)
        {
            // Khởi động Timer đếm thời gian chờ mỗi giây 1 lần
            searchTimer = new Timer();
            searchTimer.Interval = 1000;
            searchTimer.Tick += SearchTimer_Tick;
            searchTimer.Start();

            waitingSeconds = 0;
            lblStatus.Text = "Đang tìm kiếm đối thủ phù hợp...";

            // TODO: Gửi yêu cầu gia nhập hàng đợi tìm trận lên Server qua SignalR
            // Ví dụ: await hubConnection.InvokeAsync("JoinMatchmakingQueue");
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            waitingSeconds++;
            lblTimer.Text = $"Thời gian tìm: {waitingSeconds}s";
        }

        // Nút Hủy tìm trận
        private void btnCancel_Click(object sender, EventArgs e)
        {
            StopMatchmaking();
        }

        private void StopMatchmaking()
        {
            if (searchTimer != null)
            {
                searchTimer.Stop();
            }

            // TODO: Gửi yêu cầu rời hàng đợi lên Server qua SignalR
            // Ví dụ: await hubConnection.InvokeAsync("LeaveMatchmakingQueue");

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Được gọi từ SERVER khi đã tìm thấy đối thủ ghép cặp
        public void HandleMatchFound(string opponentName, int opponentElo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleMatchFound(opponentName, opponentElo)));
                return;
            }

            // Dừng đồng hồ đếm thời gian
            searchTimer.Stop();
            lblStatus.Text = $"Đã tìm thấy đối thủ: {opponentName} (Elo: {opponentElo})!\nĐang chuẩn bị vào trận...";

            // Tạo độ trễ ngắn 1.5 giây để người chơi kịp nhìn thấy thông tin đối thủ trước khi mở bàn cờ
            Timer delayTimer = new Timer();
            delayTimer.Interval = 1500;
            delayTimer.Tick += (s, args) =>
            {
                delayTimer.Stop();
                this.DialogResult = DialogResult.OK; // Báo hiệu ghép trận thành công để mở frmGame
                this.Close();
            };
            delayTimer.Start();
        }
    }
}
