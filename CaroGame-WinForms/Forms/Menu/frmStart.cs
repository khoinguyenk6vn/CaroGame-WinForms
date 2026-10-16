using CaroGame_WinForms.Forms.Account;
using CaroGame_WinForms.Services;

namespace CaroGame_WinForms.Forms.Menu
{
    // Màn hình mở app: Chơi ngay (Guest), Đăng nhập, Information, Settings
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();

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

        // Màn chọn chế độ là menu chính. Đóng menu chính là thoát game.
        private void OpenModeSelect()
        {
            Hide();
            frmModeSelect modeSelectForm = new frmModeSelect();
            modeSelectForm.ShowDialog();
            Close();
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
