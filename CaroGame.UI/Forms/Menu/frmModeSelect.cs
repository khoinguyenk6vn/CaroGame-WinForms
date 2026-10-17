using CaroGame.BLL.Dtos;
using CaroGame.BLL.Game;
using CaroGame.BLL.Services;
using CaroGame.UI.Forms.Account;
using CaroGame.UI.Forms.Match;
using CaroGame.UI.Services;
using CaroGame.BLL.Models;

namespace CaroGame.UI.Forms.Menu
{
    // Màn chọn chế độ (menu chính)
    public partial class frmModeSelect : Form
    {
        // true nếu người chơi đã đăng xuất trong Profile (frmStart sẽ hiện lại màn mở app)
        public bool LoggedOut { get; private set; }

        public frmModeSelect()
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnPvE.Click += btnPvE_Click;
            btnPvPOffline.Click += btnPvPOffline_Click;
            btnPvPOnline.Click += btnPvPOnline_Click;
            btnLeaderboard.Click += btnLeaderboard_Click;
            picAvatar.Click += picAvatar_Click;
            Load += frmModeSelect_Load;
        }

        private async void frmModeSelect_Load(object? sender, EventArgs e)
        {
            await ShowPlayerAsync();
        }

        // Hiện tên và avatar của người đang chơi
        private async Task ShowPlayerAsync()
        {
            lblAvatar.Text = Session.PlayerName;

            Image? avatar = await AvatarLoader.LoadForCurrentPlayerAsync();
            if (avatar != null && !IsDisposed)
            {
                picAvatar.Image = avatar;
            }
        }

        // ===================== PvE, Offline =====================

        private void btnPvE_Click(object? sender, EventArgs e)
        {
            OpenLocalGame(GameMode.PvE);
        }

        private void btnPvPOffline_Click(object? sender, EventArgs e)
        {
            OpenLocalGame(GameMode.PvPOffline);
        }

        // Ẩn màn chọn chế độ trong lúc chơi, đóng trận thì hiện lại
        private void OpenLocalGame(GameMode mode)
        {
            Hide();
            frmGame gameForm = new frmGame(mode, Session.PlayerName);
            gameForm.ShowDialog();
            Show();
        }

        // ===================== PvP Online =====================

        private async void btnPvPOnline_Click(object? sender, EventArgs e)
        {
            // Guest phải đăng nhập mới được chơi Online
            if (!Session.IsLoggedIn)
            {
                await OpenLoginAsync();
                return;
            }

            Hide();

            // Tìm trận -> chơi -> nếu chọn "Tìm trận mới" (hoặc đối thủ thoát sớm) thì tìm tiếp
            bool findNewMatch = true;
            while (findNewMatch)
            {
                frmMatchmaking matchmakingForm = new frmMatchmaking();
                if (matchmakingForm.ShowDialog() != DialogResult.OK || matchmakingForm.MatchInfo == null)
                {
                    break;
                }

                frmGame gameForm = new frmGame(matchmakingForm.MatchInfo);
                gameForm.ShowDialog();
                findNewMatch = gameForm.FindNewMatch;
            }

            Show();
            await ShowPlayerAsync();
        }

        // ===================== Bảng xếp hạng, Profile =====================

        private async void btnLeaderboard_Click(object? sender, EventArgs e)
        {
            // Guest phải đăng nhập mới được xem bảng xếp hạng
            if (!Session.IsLoggedIn)
            {
                await OpenLoginAsync();
                return;
            }

            frmLeaderboard leaderboardForm = new frmLeaderboard();
            leaderboardForm.ShowDialog(this);
        }

        private async void picAvatar_Click(object? sender, EventArgs e)
        {
            frmProfile profileForm = new frmProfile();
            profileForm.ShowDialog(this);

            if (profileForm.LoggedOut)
            {
                LoggedOut = true;
                Close();
                return;
            }

            // Trong Profile có thể đã đăng nhập (Guest) hoặc đổi avatar
            await ShowPlayerAsync();
        }

        private async Task OpenLoginAsync()
        {
            frmLogin loginForm = new frmLogin();
            if (loginForm.ShowDialog(this) == DialogResult.OK)
            {
                await ShowPlayerAsync();
            }
        }
    }
}
