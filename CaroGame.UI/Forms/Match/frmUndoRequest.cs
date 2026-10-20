using CaroGame.UI.Services;

namespace CaroGame.UI.Forms.Match
{
    // Hộp thoại hỏi có đồng ý cho đối thủ đi lại không.
    // Trả về DialogResult.Yes (đồng ý), DialogResult.No (từ chối),
    // hoặc DialogResult.Cancel nếu hết 15 giây và frmGame tự đóng hộp thoại.
    public partial class frmUndoRequest : Form
    {
        // Số giây còn lại để trả lời. Chỉ để hiển thị: hết giờ thì server tự tính là từ chối
        // và gửi tin "UndoTimeout", frmGame nhận tin đó rồi đóng hộp thoại này.
        private int secondsLeft = 15;

        public frmUndoRequest()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnAccept, Esc = btnDecline
            AcceptButton = btnAccept;
            CancelButton = btnDecline;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnAccept.Click += btnAccept_Click;
            btnDecline.Click += btnDecline_Click;
            tmrCountdown.Tick += tmrCountdown_Tick;

            // Bắt đầu đếm ngược (tmrCountdown chạy 1 giây/lần, Interval = 1000 trong Designer)
            lblCountdown.Text = "Còn " + secondsLeft + " giây";
            tmrCountdown.Start();
        }

        private void tmrCountdown_Tick(object? sender, EventArgs e)
        {
            if (secondsLeft > 0)
            {
                secondsLeft--;
            }
            lblCountdown.Text = "Còn " + secondsLeft + " giây";
        }

        private void btnAccept_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnDecline_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
