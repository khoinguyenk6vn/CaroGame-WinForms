using System.IO;
using System.Windows.Media;
using CaroGame.BLL.Models;
using CaroGame.BLL.Settings;

namespace CaroGame.UI.Services
{
    // Phát âm thanh và nhạc nền. File đặt trong thư mục Sounds cạnh file .exe:
    //   move.mp3  - tiếng đánh quân
    //   music.mp3 - nhạc nền, phát lặp lại
    // Thiếu file thì không phát gì, game vẫn chạy bình thường, không báo lỗi.
    // Dùng MediaPlayer (có sẵn trong .NET) vì SoundPlayer của WinForms chỉ phát được 1 âm thanh một lúc,
    // phát tiếng đánh quân sẽ làm tắt nhạc nền.
    public static class SoundManager
    {
        private static readonly MediaPlayer effectPlayer = new MediaPlayer();
        private static readonly MediaPlayer musicPlayer = new MediaPlayer();
        private static bool isMusicOpened;

        public static void PlayMoveSound()
        {
            string path = GetSoundPath("move.mp3");
            if (!SoundSettings.SoundOn || !File.Exists(path))
            {
                return;
            }

            effectPlayer.Open(new Uri(path));
            effectPlayer.Play();
        }

        // Gọi khi mở app và mỗi lần bấm nút Nhạc nền: đang bật thì phát, đang tắt thì dừng
        public static void UpdateMusic()
        {
            if (!SoundSettings.MusicOn)
            {
                musicPlayer.Stop();
                return;
            }

            string path = GetSoundPath("music.mp3");
            if (!File.Exists(path))
            {
                return;
            }

            if (!isMusicOpened)
            {
                musicPlayer.Open(new Uri(path));
                musicPlayer.MediaEnded += MusicPlayer_MediaEnded;
                isMusicOpened = true;
            }
            musicPlayer.Play();
        }

        // Hết bài thì phát lại từ đầu
        private static void MusicPlayer_MediaEnded(object? sender, EventArgs e)
        {
            musicPlayer.Position = TimeSpan.Zero;
            musicPlayer.Play();
        }

        private static string GetSoundPath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, "Sounds", fileName);
        }
    }
}
