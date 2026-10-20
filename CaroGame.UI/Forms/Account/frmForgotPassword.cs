using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Quên mật khẩu: nhập email -> nhập mã -> đặt mật khẩu mới.
    // Lỗi hiện bằng ErrorProvider: biểu tượng đỏ cạnh ô bị lỗi, rê chuột vào biểu tượng để xem lý do.
    public partial class frmForgotPassword : Form
    {
        public frmForgotPassword()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnSendCode, Esc = btnBack
            AcceptButton = btnSendCode;
            CancelButton = btnBack;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnSendCode.Click += btnSendCode_Click;
            btnBack.Click += btnBack_Click;
        }

        private async void btnSendCode_Click(object? sender, EventArgs e)
        {
            // Xóa các lỗi của lần bấm trước
            errorProvider.Clear();

            string email = txtEmail.Text.Trim();
            if (email == "")
            {
                errorProvider.SetError(txtEmail, "Chưa nhập email.");
                return;
            }

            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("ForgotPassword", email);
            }
            catch (Exception)
            {
                errorProvider.SetError(btnSendCode, "Không kết nối được server.");
                return;
            }

            // Email không tồn tại, chưa đủ 60 giây, gửi mail lỗi... đều báo cạnh ô email
            if (!result.Success)
            {
                errorProvider.SetError(txtEmail, result.Message);
                return;
            }

            // Mã đúng thì mới cho mở form đặt mật khẩu mới
            frmVerifyCode verifyForm = new frmVerifyCode(email, CodePurpose.ResetPassword);
            if (verifyForm.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            frmResetPassword resetForm = new frmResetPassword(email, verifyForm.VerifiedCode);
            if (resetForm.ShowDialog(this) == DialogResult.OK)
            {
                Close();
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
