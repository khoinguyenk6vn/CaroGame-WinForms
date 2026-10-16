namespace CaroGame_WinForms.Services
{
    // Bật/tắt âm thanh và nhạc nền, chỉnh trong form Settings.
    // Phần phát âm thanh sẽ làm ở bước 8, lúc đó đọc 2 biến này để biết có phát hay không.
    public static class SoundSettings
    {
        public static bool SoundOn { get; set; } = true;
        public static bool MusicOn { get; set; } = true;
    }
}
