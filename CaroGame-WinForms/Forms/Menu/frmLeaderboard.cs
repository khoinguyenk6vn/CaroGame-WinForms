using CaroGame.BLL.Dtos;
using CaroGame_WinForms.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Forms.Menu
{
    // Bảng xếp hạng: top 50, dòng cuối là thứ hạng của chính mình (server đã sắp sẵn)
    public partial class frmLeaderboard : Form
    {
        public frmLeaderboard()
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnBack.Click += btnBack_Click;
            Load += frmLeaderboard_Load;
        }

        private async void frmLeaderboard_Load(object? sender, EventArgs e)
        {
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                List<LeaderboardRowDto> rows = await hub.InvokeAsync<List<LeaderboardRowDto>>("GetLeaderboard");

                // DataGridView tự tạo cột theo các thuộc tính của LeaderboardRowDto (tên cột lấy từ DisplayName)
                dgvLeaderboard.AllowUserToAddRows = false;
                dgvLeaderboard.DataSource = rows;
            }
            catch (Exception)
            {
                MessageBox.Show("Không kết nối được server.", "Bảng xếp hạng");
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
