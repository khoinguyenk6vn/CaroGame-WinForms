using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;
using CaroGame.UI.Forms.Account;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Menu
{
    // Profile: avatar, tên, điểm, bậc rank, lịch sử đấu, đổi avatar, đăng xuất.
    // Guest chỉ thấy tên và nút "Đăng nhập để lưu lịch sử".
    public partial class frmProfile : Form
    {
        // true nếu người chơi bấm Đăng xuất
        public bool LoggedOut { get; private set; }

        public frmProfile()
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnChangeAvatar.Click += btnChangeAvatar_Click;
            btnHistory.Click += btnHistory_Click;
            btnLogout.Click += btnLogout_Click;
            btnLoginToSave.Click += btnLoginToSave_Click;
            btnBack.Click += btnBack_Click;
            Load += frmProfile_Load;
        }

        private async void frmProfile_Load(object? sender, EventArgs e)
        {
            await ShowProfileAsync();
        }

        private async Task ShowProfileAsync()
        {
            bool loggedIn = Session.IsLoggedIn;

            // Guest chỉ thấy tên và nút đăng nhập
            lblTitle2.Visible = loggedIn;
            lblScore.Visible = loggedIn;
            lblRank.Visible = loggedIn;
            btnChangeAvatar.Visible = loggedIn;
            btnHistory.Visible = loggedIn;
            btnLogout.Visible = loggedIn;
            btnLoginToSave.Visible = !loggedIn;

            lblUsername.Text = Session.PlayerName;

            if (loggedIn)
            {
                try
                {
                    HubConnection hub = await ServerConnection.ConnectAsync();
                    ProfileDto? profile = await hub.InvokeAsync<ProfileDto?>("GetProfile");
                    if (profile != null)
                    {
                        Session.UpdateProfile(profile);
                        lblScore.Text = profile.Score.ToString();
                        lblRank.Text = profile.RankName;
                    }
                }
                catch (Exception)
                {
                    lblRank.Text = "Không kết nối được server.";
                }
            }

            Image? avatar = await AvatarLoader.LoadForCurrentPlayerAsync();
            if (avatar != null && !IsDisposed)
            {
                pictureBox1.Image = avatar;
            }
        }

        private async void btnChangeAvatar_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog();
            dialog.Filter = "Ảnh (*.png;*.jpg)|*.png;*.jpg";
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            // Kiểm tra đuôi file và dung lượng tối đa 1MB do server (tầng BLL) làm
            string message = await AvatarLoader.UploadAsync(dialog.FileName);
            MessageBox.Show(message, "Đổi avatar");
            await ShowProfileAsync();
        }

        private void btnHistory_Click(object? sender, EventArgs e)
        {
            frmHistory historyForm = new frmHistory();
            historyForm.ShowDialog(this);
        }

        private async void btnLogout_Click(object? sender, EventArgs e)
        {
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                await hub.InvokeAsync("Logout");
            }
            catch (Exception)
            {
                // Không kết nối được thì server cũng sẽ tự xóa phiên khi mất kết nối
            }

            Session.StartAsGuest();
            LoggedOut = true;
            Close();
        }

        private async void btnLoginToSave_Click(object? sender, EventArgs e)
        {
            frmLogin loginForm = new frmLogin();
            if (loginForm.ShowDialog(this) == DialogResult.OK)
            {
                await ShowProfileAsync();
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
