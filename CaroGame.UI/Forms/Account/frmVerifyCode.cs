using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Account
{
    // Nhập mã 6 số gửi qua email. Dùng chung cho đăng ký và quên mật khẩu.
    // Trả về DialogResult.OK nếu mã đúng.
    // Lỗi hiện bằng ErrorProvider: biểu tượng đỏ cạnh ô/nút bị lỗi, rê chuột vào biểu tượng để xem lý do.
    public partial class frmVerifyCode : Form
    {
        private readonly string email;
        private readonly CodePurpose purpose;

        // Mã người chơi đã nhập đúng. Quên mật khẩu cần gửi lại mã này khi đổi mật khẩu.
        public string VerifiedCode { get; private set; } = "";

        public frmVerifyCode(string email, CodePurpose purpose)
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnConfirm, Esc = btnBack
            AcceptButton = btnConfirm;
            CancelButton = btnBack;
            this.email = email;
            this.purpose = purpose;

            lblMessage.Text = "Mã xác minh đã được gửi tới " + email;
            lblResendCountdown.Text = "";

            // Gắn các sự kiện chưa được gắn trong Designer
            btnConfirm.Click += btnConfirm_Click;
            btnResend.Click += btnResend_Click;
            btnBack.Click += btnBack_Click;
        }

        private async void btnConfirm_Click(object? sender, EventArgs e)
        {
            // Xóa các lỗi của lần bấm trước
            errorProvider.Clear();

            string code = txtCode.Text.Trim();
            if (code == "")
            {
                errorProvider.SetError(txtCode, "Chưa nhập mã.");
                return;
            }

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
                errorProvider.SetError(btnConfirm, "Không kết nối được server.");
                return;
            }

            // Mã sai, hết hạn, bị hủy... đều là lỗi của ô nhập mã
            if (!result.Success)
            {
                errorProvider.SetError(txtCode, result.Message);
                return;
            }

            VerifiedCode = code;
            DialogResult = DialogResult.OK;
        }

        // Server kiểm tra luật chờ 60 giây giữa 2 lần gửi và trả về thông báo
        private async void btnResend_Click(object? sender, EventArgs e)
        {
            errorProvider.Clear();
            lblResendCountdown.Text = "";

            AccountResult result;
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                result = await hub.InvokeAsync<AccountResult>("ResendCode", email, purpose);
            }
            catch (Exception)
            {
                errorProvider.SetError(btnResend, "Không kết nối được server.");
                return;
            }

            // Gửi được thì hiện thông báo, không gửi được (ví dụ chưa đủ 60 giây) thì báo lỗi cạnh nút
            if (result.Success)
            {
                lblResendCountdown.Text = result.Message;
            }
            else
            {
                errorProvider.SetError(btnResend, result.Message);
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
