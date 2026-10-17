namespace CaroGame.BLL.Settings
{
    // Bật/tắt âm thanh và nhạc nền, chỉnh trong form Settings.
    // Phần phát âm thanh sẽ làm ở bước 8, lúc đó đọc 2 biến này để biết có phát hay không.
    public static class SoundSettings
    {
        public static bool SoundOn { get; private set; } = true;
        public static bool MusicOn { get; private set; } = true;

        // Mỗi lần bấm nút thì đổi bật thành tắt và ngược lại
        public static void ToggleSound()
        {
            SoundOn = !SoundOn;
        }

        public static void ToggleMusic()
        {
            MusicOn = !MusicOn;
        }
    }
}
