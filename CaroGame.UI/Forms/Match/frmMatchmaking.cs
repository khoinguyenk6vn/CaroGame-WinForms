using CaroGame.BLL.Dtos;
using CaroGame.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace CaroGame.UI.Forms.Match
{
    // Màn chờ ghép trận Online. Trả về DialogResult.OK kèm MatchInfo khi đã ghép được trận.
    public partial class frmMatchmaking : Form
    {
        // Thông tin trận vừa ghép được (null nếu chưa ghép được)
        public MatchFoundDto? MatchInfo { get; private set; }

        private HubConnection? hub;

        public frmMatchmaking()
        {
            InitializeComponent();
            FormClosing += frmMatchmaking_FormClosing;
        }

        private async void frmMatchmaking_Load(object? sender, EventArgs e)
        {
            lblStatus.Text = "Đang tìm đối thủ phù hợp...";
            lblTimer.Text = "Thời gian tìm: 0s";

            try
            {
                hub = await ServerConnection.ConnectAsync();

                // Tin từ server đến trên luồng khác, không phải luồng giao diện.
                // Control chỉ được sửa trên luồng giao diện, nên phải đưa việc cập nhật qua BeginInvoke.
                hub.On<int>("SearchTime", seconds => BeginInvoke(new Action(() => ShowSearchTime(seconds))));
                hub.On<MatchFoundDto>("MatchFound", info => BeginInvoke(new Action(() => OnMatchFound(info))));

                string error = await hub.InvokeAsync<string>("JoinQueue");
                if (error != "")
                {
                    lblStatus.Text = error;
                }
            }
            catch (Exception)
            {
                lblStatus.Text = "Không kết nối được server.";
            }
        }

        private void ShowSearchTime(int seconds)
        {
            lblTimer.Text = "Thời gian tìm: " + seconds + "s";
        }

        private void OnMatchFound(MatchFoundDto info)
        {
            MatchInfo = info;
            DialogResult = DialogResult.OK;
        }

        // Nút Dừng tìm trận
        private void btnCancel_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private async void frmMatchmaking_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (hub == null)
            {
                return;
            }

            hub.Remove("SearchTime");
            hub.Remove("MatchFound");

            // Đóng màn này mà chưa ghép được trận thì rời hàng chờ
            if (MatchInfo == null)
            {
                try
                {
                    await hub.InvokeAsync("LeaveQueue");
                }
                catch (Exception)
                {
                    // Mất kết nối thì server tự xóa khỏi hàng chờ
                }
            }
        }
    }
}
