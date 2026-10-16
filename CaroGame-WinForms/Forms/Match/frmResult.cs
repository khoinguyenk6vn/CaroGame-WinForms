using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CaroGame_WinForms
{
    public partial class frmResult : Form
    {
        //public frmResult()
        //{
        //    InitializeComponent();
        //}
        // Hàm khởi tạo nhận tham số kết quả từ màn hình trận đấu truyền sang
        // isWin: true nếu thắng, false nếu thua/hòa
        // gameMode: "PvP Online", "PvE", hoặc "PvP Offline"
        // eloChange: số điểm cộng/trừ (ví dụ: +50, -50, 0)
        public frmResult(bool isWin, string gameMode, int eloChange)
        {
            InitializeComponent();
            LoadResultData(isWin, gameMode, eloChange);
        }

        private void LoadResultData(bool isWin, string gameMode, int eloChange)
        {
            // Hiển thị chế độ chơi
            lblGameMode.Text = "Chế độ: " + gameMode;

            // Thiết lập trạng thái Thắng / Thua kèm màu sắc trực quan
            if (isWin)
            {
                lblResultStatus.Text = "CHIẾN THẮNG!";
                lblResultStatus.ForeColor = Color.ForestGreen;
            }
            else
            {
                lblResultStatus.Text = "THẤT BẠIIII";
                lblResultStatus.ForeColor = Color.Crimson;
            }

            // Theo yêu cầu đồ án: Trận PvP Online mới hiện điểm cộng/trừ, PvE/Offline không hiện điểm
            if (gameMode == "PvP Online")
            {
                lblEloChange.Visible = true;
                if (eloChange > 0)
                {
                    lblEloChange.Text = $"Điểm thay đổi: +{eloChange} Elo";
                    lblEloChange.ForeColor = Color.Blue;
                }
                else if (eloChange < 0)
                {
                    lblEloChange.Text = $"Điểm thay đổi: {eloChange} Elo";
                    lblEloChange.ForeColor = Color.OrangeRed;
                }
                else
                {
                    lblEloChange.Text = "Điểm thay đổi: 0 Elo (Hòa)";
                    lblEloChange.ForeColor = Color.Gray;
                }
            }
            else
            {
                lblEloChange.Visible = false; // Ẩn đi nếu là PvE hoặc PvP Offline
            }
        }

        // Nút chơi lại: Trả về kết quả DialogResult.Retry để form gọi biết đường xử lý
        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        // Nút về menu chính: Trả về kết quả DialogResult.Cancel để đóng form game
        private void btnBackToMenu_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
