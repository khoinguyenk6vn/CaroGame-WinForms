using CaroGame_WinForms.Forms.Menu;
using CaroGame_WinForms.Services;

namespace CaroGame_WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Phát nhạc nền nếu đang bật và có file Sounds/music.mp3
            SoundManager.UpdateMusic();

            Application.Run(new frmStart());
        }
    }
}
