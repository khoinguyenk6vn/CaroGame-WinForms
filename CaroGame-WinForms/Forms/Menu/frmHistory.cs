using CaroGame.BLL.Dtos;
using CaroGame_WinForms.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame_WinForms.Forms.Menu
{
    // Lịch sử đấu: trận Online có điểm cộng/trừ, PvE/Offline chỉ hiện chế độ
    public partial class frmHistory : Form
    {
        public frmHistory()
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnBack.Click += btnBack_Click;
            Load += frmHistory_Load;
        }

        private async void frmHistory_Load(object? sender, EventArgs e)
        {
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                List<HistoryItemDto> items = await hub.InvokeAsync<List<HistoryItemDto>>("GetHistory");

                // DataGridView tự tạo cột theo các thuộc tính của HistoryItemDto (tên cột lấy từ DisplayName)
                dgvHistory.AllowUserToAddRows = false;
                dgvHistory.DataSource = items;
            }
            catch (Exception)
            {
                MessageBox.Show("Không kết nối được server.", "Lịch sử đấu");
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
