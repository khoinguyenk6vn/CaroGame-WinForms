using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Quên mật khẩu: nhập email -> nhập mã -> đặt mật khẩu mới
    public partial class frmForgotPassword : Form
    {
        public frmForgotPassword()
        {
            InitializeComponent();
            lblError.Text = "";

            // Gắn các sự kiện chưa được gắn trong Designer
            btnSendCode.Click += btnSendCode_Click;
            btnBack.Click += btnBack_Click;
        }

        private async void btnSendCode_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";
            string email = txtEmail.Text.Trim();

            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("ForgotPassword", email);
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
