using CaroGame.UI.Forms.Account;
using CaroGame.BLL.Services;
using CaroGame.UI.Services;

namespace CaroGame.UI.Forms.Menu
{
    // Màn hình mở app: Chơi ngay (Guest), Đăng nhập, Information, Settings
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Enter = btnPlayNow
            AcceptButton = btnPlayNow;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnPlayNow.Click += btnPlayNow_Click;
            btnLogin.Click += btnLogin_Click;
            toolStripButton1.Click += btnSettings_Click;     // icon bánh răng
            toolStripButton2.Click += btnInformation_Click;  // icon chữ i
        }

        // Chơi ngay: vào thẳng màn chọn chế độ với tên Guest tự sinh
        private void btnPlayNow_Click(object? sender, EventArgs e)
        {
            Session.StartAsGuest();
            OpenModeSelect();
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            frmLogin loginForm = new frmLogin();
            if (loginForm.ShowDialog(this) == DialogResult.OK)
            {
                OpenModeSelect();
            }
        }

        // Màn chọn chế độ là menu chính. Đóng menu chính là thoát game,
        // còn đăng xuất (trong Profile) thì quay lại màn hình mở app.
        private void OpenModeSelect()
        {
            Hide();
            frmModeSelect modeSelectForm = new frmModeSelect();
            modeSelectForm.ShowDialog();

            if (modeSelectForm.LoggedOut)
            {
                Show();
            }
            else
            {
                Close();
            }
        }

        private void btnSettings_Click(object? sender, EventArgs e)
        {
            frmSettings settingsForm = new frmSettings();
            settingsForm.ShowDialog(this);
        }

        private void btnInformation_Click(object? sender, EventArgs e)
        {
            frmInformation informationForm = new frmInformation();
            informationForm.ShowDialog(this);
        }
    }
}
