using CaroGame.UI.Services;

namespace CaroGame.UI.Forms.Match
{
    // Hộp thoại kết quả trận.
    // Trả về DialogResult.Retry nếu chọn "Chơi lại"/"Tìm trận mới", DialogResult.Cancel nếu chọn "Về menu".
    public partial class frmResult : Form
    {
        // scoreText: chữ điểm thay đổi (chỉ trận Online có), để rỗng thì ẩn
        // playAgainText: "Chơi lại" (PvE/Offline) hoặc "Tìm trận mới" (Online)
        public frmResult(string resultText, string modeText, string reasonText, string scoreText, string playAgainText)
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnPlayAgain, Esc = btnBackToMenu
            AcceptButton = btnPlayAgain;
            CancelButton = btnBackToMenu;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnPlayAgain.Click += btnPlayAgain_Click;
            btnBackToMenu.Click += btnBackToMenu_Click;

            lblResultStatus.Text = resultText;
            lblGameMode.Text = "Chế độ: " + modeText;

            // Tạm hiện lý do kết thúc trên thanh tiêu đề, khi nhóm thêm lblReason thì chuyển sang đó
            Text = "Lý do: " + reasonText;

            lblEloChange.Visible = scoreText != "";
            lblEloChange.Text = scoreText;

            btnPlayAgain.Text = playAgainText;
        }

        // Gán DialogResult cho Form mở bằng ShowDialog thì Form tự đóng
        private void btnPlayAgain_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }

        private void btnBackToMenu_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
