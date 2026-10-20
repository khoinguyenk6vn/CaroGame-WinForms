using CaroGame.BLL.Dtos;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Menu
{
    // Bảng xếp hạng: top 50, dòng cuối là thứ hạng của chính mình (server đã sắp sẵn)
    public partial class frmLeaderboard : Form
    {
        // DataGridView lấy dữ liệu qua BindingSource: đổi DataSource của BindingSource là bảng tự hiện lại
        private readonly BindingSource leaderboardSource = new BindingSource();

        public frmLeaderboard()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Esc = btnBack
            CancelButton = btnBack;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnBack.Click += btnBack_Click;
            Load += frmLeaderboard_Load;

            // Bảng chỉ để xem, không cho thêm dòng
            dgvLeaderboard.AllowUserToAddRows = false;
            dgvLeaderboard.DataSource = leaderboardSource;
        }

        private async void frmLeaderboard_Load(object? sender, EventArgs e)
        {
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                List<LeaderboardRowDto> rows = await hub.InvokeAsync<List<LeaderboardRowDto>>("GetLeaderboard");

                // DataGridView tự tạo cột theo các thuộc tính của LeaderboardRowDto (tên cột lấy từ DisplayName)
                leaderboardSource.DataSource = rows;
            }
            catch (Exception)
            {
                // Hiện biểu tượng đỏ cạnh bảng, rê chuột vào để xem lý do
                errorProvider.SetError(dgvLeaderboard, "Không kết nối được server.");
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
