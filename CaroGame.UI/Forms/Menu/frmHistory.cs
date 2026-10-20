using CaroGame.BLL.Dtos;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Menu
{
    // Lịch sử đấu: trận Online có điểm cộng/trừ, PvE/Offline chỉ hiện chế độ
    public partial class frmHistory : Form
    {
        // DataGridView lấy dữ liệu qua BindingSource: đổi DataSource của BindingSource là bảng tự hiện lại
        private readonly BindingSource historySource = new BindingSource();

        public frmHistory()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Esc = btnBack
            CancelButton = btnBack;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnBack.Click += btnBack_Click;
            Load += frmHistory_Load;

            // Bảng chỉ để xem, không cho thêm dòng
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.DataSource = historySource;
        }

        private async void frmHistory_Load(object? sender, EventArgs e)
        {
            try
            {
                HubConnection hub = await ServerConnection.ConnectAsync();
                List<HistoryItemDto> items = await hub.InvokeAsync<List<HistoryItemDto>>("GetHistory");

                // DataGridView tự tạo cột theo các thuộc tính của HistoryItemDto (tên cột lấy từ DisplayName)
                historySource.DataSource = items;
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
