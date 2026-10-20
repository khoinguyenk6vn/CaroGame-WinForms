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

        // Thông tin Profile lấy từ server. lblScore, lblRank gắn với BindingSource này (DataBindings),
        // nên mỗi lần gán ProfileDto mới vào DataSource thì 2 label tự hiện điểm và bậc rank mới.
        private readonly BindingSource profileSource = new BindingSource();

        public frmProfile()
        {
            InitializeComponent();
            new FormScaler(this);

            // Chưa có dữ liệu thì cho BindingSource biết kiểu dữ liệu (ProfileDto) để gắn được tên thuộc tính.
            // "Text" là thuộc tính của Label, "Score"/"RankName" là thuộc tính của ProfileDto.
            profileSource.DataSource = typeof(ProfileDto);
            lblScore.DataBindings.Add("Text", profileSource, "Score");
            lblRank.DataBindings.Add("Text", profileSource, "RankName");

            // Phím tắt: Esc = btnBack
            CancelButton = btnBack;

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
            picRank.Visible = loggedIn;
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
                        profileSource.DataSource = profile;

                        // Logo rank theo điểm hiện tại
                        picRank.Image = RankIconLoader.Load(profile.Score);
                    }
                }
                catch (Exception)
                {
                    errorProvider.SetError(lblRank, "Không kết nối được server.");
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

            errorProvider.Clear();

            // Kiểm tra đuôi file và dung lượng tối đa 1MB do server (tầng BLL) làm
            AccountResult result = await AvatarLoader.UploadAsync(dialog.FileName);
            if (!result.Success)
            {
                // Lỗi: hiện biểu tượng đỏ cạnh nút đổi avatar, rê chuột vào để xem lý do
                errorProvider.SetError(btnChangeAvatar, result.Message);
                return;
            }

            MessageBox.Show(result.Message, "Đổi avatar");
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
