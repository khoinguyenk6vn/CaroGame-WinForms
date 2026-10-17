using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame_WinForms.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Forms.Account
{
    // Đặt mật khẩu mới sau khi đã nhập đúng mã. Trả về DialogResult.OK nếu đổi thành công.
    public partial class frmResetPassword : Form
    {
        private readonly string email;
        private readonly string code;

        public frmResetPassword(string email, string code)
        {
            InitializeComponent();
            this.email = email;
            this.code = code;
            lblError.Text = "";

            // Gắn các sự kiện chưa được gắn trong Designer
            btnConfirm.Click += btnConfirm_Click;
            btnBack.Click += btnBack_Click;
        }

        private async void btnConfirm_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";

            // Gửi kèm mã để server kiểm tra lại lần nữa trước khi đổi mật khẩu.
            // Độ dài mật khẩu và nhập lại có khớp không do tầng BLL ở server kiểm tra.
            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("ResetPassword", email, code, txtNewPassword.Text, txtConfirmPassword.Text);
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

            MessageBox.Show("Đổi mật khẩu thành công. Hãy đăng nhập bằng mật khẩu mới.", "Quên mật khẩu");
            DialogResult = DialogResult.OK;
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
