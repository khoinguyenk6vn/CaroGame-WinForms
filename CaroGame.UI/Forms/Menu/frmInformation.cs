using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CaroGame.UI.Services;

namespace CaroGame.UI.Forms.Menu
{
    public partial class frmInformation : Form
    {
        public frmInformation()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Esc = btnClose
            CancelButton = btnClose;

            btnClose.Click += btnClose_Click;
            rtbGuide.Text = GuideText;
        }

        // Nội dung hướng dẫn cho người mới, hiện trong ô cuộn dọc
        private const string GuideText =
            "CÁCH CHƠI CARO\n\n" +
            "- Bàn cờ 20x20. X luôn đi trước, 2 người lần lượt đặt quân.\n" +
            "- Ai có từ 5 quân liền nhau trở lên (ngang, dọc hoặc chéo) là thắng.\n" +
            "- Luật chặn 2 đầu: hàng bị quân đối thủ chặn sát cả 2 đầu thì không tính thắng. Mép bàn cờ không tính là chặn.\n" +
            "- Bàn cờ đầy mà chưa ai thắng thì hòa.\n\n" +
            "CHẾ ĐỘ CHƠI\n\n" +
            "- PvE: đánh với máy. Bạn cầm X. Không tính giờ. Có thể chat với bot.\n" +
            "- PvP Offline: 2 người chơi chung 1 máy. Mỗi người có 180 giây cho cả trận, hết giờ là thua.\n" +
            "- PvP Online: đấu xếp hạng qua mạng, phải đăng nhập. Mỗi người 180 giây. " +
            "Trước trận, cả 2 người được hỏi có dùng luật chặn 2 đầu không.\n\n" +
            "NÚT TRONG TRẬN\n\n" +
            "- Undo: đi lại. PvE lùi 2 nước, Offline lùi 1 nước. " +
            "Online phải được đối thủ đồng ý, bị từ chối 3 lần thì không được xin nữa.\n" +
            "- FF: đầu hàng. Đóng màn trận khi chưa hết trận cũng tính là đầu hàng.\n\n" +
            "ĐIỂM XẾP HẠNG (CHỈ ONLINE)\n\n" +
            "- Điểm bắt đầu là 1000 và không bao giờ dưới 1000.\n" +
            "- Thắng nhanh được nhiều điểm hơn, đầu hàng hoặc mất kết nối quá 30 giây bị trừ 100 điểm.\n" +
            "- 9 hạng từ thấp lên cao: Sắt, Đồng, Bạc, Vàng, Bạch Kim, Kim Cương, Lục Bảo, Trái Đất, Mặt Trời. " +
            "Mỗi hạng có 3 bậc I, II, III.";

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
