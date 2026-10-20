using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Đặt mật khẩu mới sau khi đã nhập đúng mã. Trả về DialogResult.OK nếu đổi thành công.
    // Lỗi hiện bằng ErrorProvider: biểu tượng đỏ cạnh ô/nút bị lỗi, rê chuột vào biểu tượng để xem lý do.
    public partial class frmResetPassword : Form
    {
        private readonly string email;
        private readonly string code;

        public frmResetPassword(string email, string code)
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnConfirm, Esc = btnBack
            AcceptButton = btnConfirm;
            CancelButton = btnBack;
            this.email = email;
            this.code = code;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnConfirm.Click += btnConfirm_Click;
            btnBack.Click += btnBack_Click;
        }

        private async void btnConfirm_Click(object? sender, EventArgs e)
        {
            // Xóa các lỗi của lần bấm trước
            errorProvider.Clear();

            // Ô nào để trống thì báo lỗi cạnh ô đó, chưa gửi lên server
            bool hasEmpty = false;
            if (txtNewPassword.Text == "")
            {
                errorProvider.SetError(txtNewPassword, "Chưa nhập mật khẩu mới.");
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
                errorProvider.SetError(btnConfirm, "Không kết nối được server.");
                return;
            }

            if (!result.Success)
            {
                errorProvider.SetError(btnConfirm, result.Message);
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
