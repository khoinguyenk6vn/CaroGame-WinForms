using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Đăng nhập. Trả về DialogResult.OK nếu đăng nhập thành công.
    // Lỗi hiện bằng ErrorProvider: biểu tượng đỏ cạnh ô/nút bị lỗi, rê chuột vào biểu tượng để xem lý do.
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnContinue, Esc = btnBack
            AcceptButton = btnContinue;
            CancelButton = btnBack;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnContinue.Click += btnContinue_Click;
            btnBack.Click += btnBack_Click;
            lnkRegister.LinkClicked += lnkRegister_LinkClicked;
            lnkForgotPassword.LinkClicked += lnkForgotPassword_LinkClicked;
        }

        private async void btnContinue_Click(object? sender, EventArgs e)
        {
            // Xóa các lỗi của lần bấm trước
            errorProvider.Clear();

            // Ô nào để trống thì báo lỗi cạnh ô đó, chưa gửi lên server
            bool hasEmpty = false;
            if (txtUsername.Text.Trim() == "")
            {
                errorProvider.SetError(txtUsername, "Chưa nhập username.");
                hasEmpty = true;
            }
            if (txtPassword.Text == "")
            {
                errorProvider.SetError(txtPassword, "Chưa nhập mật khẩu.");
                hasEmpty = true;
            }
            if (hasEmpty)
            {
                return;
            }

            LoginResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<LoginResult>("Login", txtUsername.Text.Trim(), txtPassword.Text);
            }
            catch (Exception)
            {
                errorProvider.SetError(btnContinue, "Không kết nối được server.");
                return;
            }

            // Tài khoản chưa xác minh email: server đã gửi mã, chuyển sang form nhập mã
            if (result.NeedEmailVerification)
            {
                frmVerifyCode verifyForm = new frmVerifyCode(result.Email, CodePurpose.Register);
                if (verifyForm.ShowDialog(this) == DialogResult.OK)
                {
                    MessageBox.Show("Xác minh thành công, hãy bấm Tiếp tục để đăng nhập.", "Đăng nhập");
                }
                return;
            }

            if (!result.Success)
            {
                errorProvider.SetError(btnContinue, result.Message);
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
