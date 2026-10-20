using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Đăng ký tài khoản, sau đó bắt nhập mã xác minh gửi qua email.
    // Lỗi hiện bằng ErrorProvider: biểu tượng đỏ cạnh ô/nút bị lỗi, rê chuột vào biểu tượng để xem lý do.
    public partial class frmRegister : Form
    {
        public frmRegister()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnContinue, Esc = btnBack
            AcceptButton = btnContinue;
            CancelButton = btnBack;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnContinue.Click += btnContinue_Click;
            btnBack.Click += btnBack_Click;
            lnkLogin.LinkClicked += lnkLogin_LinkClicked;
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
            if (txtEmail.Text.Trim() == "")
            {
                errorProvider.SetError(txtEmail, "Chưa nhập email.");
                hasEmpty = true;
            }
            if (txtPassword.Text == "")
            {
                errorProvider.SetError(txtPassword, "Chưa nhập mật khẩu.");
                hasEmpty = true;
            }
            if (txtConfirmPassword.Text == "")
            {
                errorProvider.SetError(txtConfirmPassword, "Chưa nhập lại mật khẩu.");
                hasEmpty = true;
            }
            if (hasEmpty)
            {
                return;
            }

            // Các điều kiện còn lại (độ dài username, mật khẩu, nhập lại khớp, email hợp lệ, trùng tên) do tầng BLL ở server kiểm tra
            string email = txtEmail.Text.Trim();
            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("Register", txtUsername.Text.Trim(), email, txtPassword.Text, txtConfirmPassword.Text);
            }
            catch (Exception)
            {
                errorProvider.SetError(btnContinue, "Không kết nối được server.");
                return;
            }

            if (!result.Success)
            {
                errorProvider.SetError(btnContinue, result.Message);
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
