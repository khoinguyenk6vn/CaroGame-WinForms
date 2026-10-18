using CaroGame.UI.Services;
using CaroGame.BLL.Settings;

namespace CaroGame.UI.Forms.Menu
{
    // Cài đặt: chỉ gồm bật/tắt âm thanh và nhạc nền
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
            new FormScaler(this);

            // Phím tắt: Esc = btnClose
            CancelButton = btnClose;

            // Gắn các sự kiện chưa được gắn trong Designer
            btnSound.Click += btnSound_Click;
            btnMusic.Click += btnMusic_Click;
            btnClose.Click += btnClose_Click;

            ShowButtonTexts();
        }

        private void btnSound_Click(object? sender, EventArgs e)
        {
            SoundSettings.ToggleSound();
            ShowButtonTexts();
        }

        private void btnMusic_Click(object? sender, EventArgs e)
        {
            SoundSettings.ToggleMusic();
            SoundManager.UpdateMusic();
            ShowButtonTexts();
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }

        // Chữ trên nút cho biết đang bật hay tắt
        private void ShowButtonTexts()
        {
            btnSound.Text = SoundSettings.SoundOn ? "Âm thanh: Bật" : "Âm thanh: Tắt";
            btnMusic.Text = SoundSettings.MusicOn ? "Nhạc nền: Bật" : "Nhạc nền: Tắt";
        }
    }
}
