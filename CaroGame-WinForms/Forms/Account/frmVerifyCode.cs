using CaroGame.Shared.Dtos;
using CaroGame_WinForms.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Forms.Account
{
    // Nhập mã 6 số gửi qua email. Dùng chung cho đăng ký và quên mật khẩu.
    // Trả về DialogResult.OK nếu mã đúng.
    public partial class frmVerifyCode : Form
    {
        private readonly string email;
        private readonly CodePurpose purpose;

        // Mã người chơi đã nhập đúng. Quên mật khẩu cần gửi lại mã này khi đổi mật khẩu.
        public string VerifiedCode { get; private set; } = "";

        public frmVerifyCode(string email, CodePurpose purpose)
        {
            InitializeComponent();
            this.email = email;
            this.purpose = purpose;

            lblMessage.Text = "Mã xác minh đã được gửi tới " + email;
            lblResendCountdown.Text = "";
            lblError.Text = "";

            // Gắn các sự kiện chưa được gắn trong Designer
            btnConfirm.Click += btnConfirm_Click;
            btnResend.Click += btnResend_Click;
            btnBack.Click += btnBack_Click;
        }

        private async void btnConfirm_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";
            string code = txtCode.Text.Trim();

            // Đăng ký: xác minh email. Quên mật khẩu: chỉ kiểm tra mã đúng chưa.
            string methodName = purpose == CodePurpose.Register ? "VerifyEmail" : "CheckResetCode";

            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>(methodName, email, code);
            }
            catch (Exception)
            {
                lblError.Text = "Không kết nối được server.";
                return;
            }

            if (!result.Success)
            {
                lblError.Text = result.Message;
                return;
            }

            VerifiedCode = code;
            DialogResult = DialogResult.OK;
        }

        // Server kiểm tra luật chờ 60 giây giữa 2 lần gửi và trả về thông báo
        private async void btnResend_Click(object? sender, EventArgs e)
        {
            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("ResendCode", email, purpose);
            }
            catch (Exception)
            {
                lblError.Text = "Không kết nối được server.";
                return;
            }

            lblResendCountdown.Text = result.Message;
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
