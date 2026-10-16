namespace CaroGame_WinForms
{
    // Hộp thoại kết quả trận.
    // Trả về DialogResult.Retry nếu chọn "Chơi lại", DialogResult.Cancel nếu chọn "Về menu".
    public partial class frmResult : Form
    {
        public frmResult(string resultText, string modeText, string reasonText)
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnPlayAgain.Click += btnPlayAgain_Click;
            btnBackToMenu.Click += btnBackToMenu_Click;

            lblResultStatus.Text = resultText;
            lblGameMode.Text = "Chế độ: " + modeText;

            // Tạm hiện lý do kết thúc trên thanh tiêu đề, khi nhóm thêm lblReason thì chuyển sang đó
            Text = "Lý do: " + reasonText;

            // PvE và Offline không có điểm
            lblEloChange.Visible = false;
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
