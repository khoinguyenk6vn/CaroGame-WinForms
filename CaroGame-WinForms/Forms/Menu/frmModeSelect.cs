using CaroGame.Shared.Game;
using CaroGame_WinForms.Forms.Account;
using CaroGame_WinForms.Services;

namespace CaroGame_WinForms.Forms.Menu
{
    // Màn chọn chế độ (menu chính)
    public partial class frmModeSelect : Form
    {
        public frmModeSelect()
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnPvE.Click += btnPvE_Click;
            btnPvPOffline.Click += btnPvPOffline_Click;
            btnPvPOnline.Click += btnPvPOnline_Click;
            btnLeaderboard.Click += btnLeaderboard_Click;

            lblAvatar.Text = Session.PlayerName;
        }

        private void btnPvE_Click(object? sender, EventArgs e)
        {
            OpenGame(GameMode.PvE);
        }

        private void btnPvPOffline_Click(object? sender, EventArgs e)
        {
            OpenGame(GameMode.PvPOffline);
        }

        // Ẩn màn chọn chế độ trong lúc chơi, đóng trận thì hiện lại
        private void OpenGame(GameMode mode)
        {
            Hide();
            frmGame gameForm = new frmGame(mode, Session.PlayerName);
            gameForm.ShowDialog();
            Show();
        }

        private void btnPvPOnline_Click(object? sender, EventArgs e)
        {
            // Guest phải đăng nhập mới được chơi Online
            if (!Session.IsLoggedIn)
            {
                OpenLogin();
                return;
            }

            // Ghép trận Online sẽ làm ở bước 6
        }

        private void btnLeaderboard_Click(object? sender, EventArgs e)
        {
            // Guest phải đăng nhập mới được xem bảng xếp hạng
            if (!Session.IsLoggedIn)
            {
                OpenLogin();
                return;
            }

            // Bảng xếp hạng sẽ làm ở bước 5
        }

        private void OpenLogin()
        {
            frmLogin loginForm = new frmLogin();
            if (loginForm.ShowDialog(this) == DialogResult.OK)
            {
                lblAvatar.Text = Session.PlayerName;
            }
        }
    }
}
