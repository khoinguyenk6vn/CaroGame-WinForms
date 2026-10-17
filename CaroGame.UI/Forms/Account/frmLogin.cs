using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Đăng nhập. Trả về DialogResult.OK nếu đăng nhập thành công.
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            lblError.Text = "";

            // Gắn các sự kiện chưa được gắn trong Designer
            btnContinue.Click += btnContinue_Click;
            btnBack.Click += btnBack_Click;
            lnkRegister.LinkClicked += lnkRegister_LinkClicked;
            lnkForgotPassword.LinkClicked += lnkForgotPassword_LinkClicked;
        }

        private async void btnContinue_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";

            LoginResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<LoginResult>("Login", txtUsername.Text.Trim(), txtPassword.Text);
            }
            catch (Exception)
            {
                lblError.Text = "Không kết nối được server.";
                return;
            }

            // Tài khoản chưa xác minh email: server đã gửi mã, chuyển sang form nhập mã
            if (result.NeedEmailVerification)
            {
                frmVerifyCode verifyForm = new frmVerifyCode(result.Email, CodePurpose.Register);
                if (verifyForm.ShowDialog(this) == DialogResult.OK)
                {
                    lblError.Text = "Xác minh thành công, hãy bấm Tiếp tục để đăng nhập.";
                }
                return;
            }

            if (!result.Success)
            {
                lblError.Text = result.Message;
                return;
            }

            Session.Login(result);
            DialogResult = DialogResult.OK;
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void lnkRegister_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            frmRegister registerForm = new frmRegister();
            registerForm.ShowDialog(this);
        }

        private void lnkForgotPassword_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            frmForgotPassword forgotForm = new frmForgotPassword();
            forgotForm.ShowDialog(this);
        }
    }
}
