using CaroGame.Shared.Dtos;
using CaroGame_WinForms.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Forms.Account
{
    // Đăng ký tài khoản, sau đó bắt nhập mã xác minh gửi qua email
    public partial class frmRegister : Form
    {
        public frmRegister()
        {
            InitializeComponent();
            lblError.Text = "";

            // Gắn các sự kiện chưa được gắn trong Designer
            btnContinue.Click += btnContinue_Click;
            btnBack.Click += btnBack_Click;
            lnkLogin.LinkClicked += lnkLogin_LinkClicked;
        }

        private async void btnContinue_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                lblError.Text = "Mật khẩu nhập lại không khớp.";
                return;
            }

            // Các điều kiện còn lại (độ dài username, mật khẩu, email hợp lệ, trùng tên) do server kiểm tra
            string email = txtEmail.Text.Trim();
            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("Register", txtUsername.Text.Trim(), email, txtPassword.Text);
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

            frmVerifyCode verifyForm = new frmVerifyCode(email, CodePurpose.Register);
            if (verifyForm.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show("Đăng ký thành công. Hãy đăng nhập.", "Đăng ký");
                Close();
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }

        // Form đăng ký được mở từ form đăng nhập, nên đóng lại là quay về đăng nhập
        private void lnkLogin_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }
    }
}
