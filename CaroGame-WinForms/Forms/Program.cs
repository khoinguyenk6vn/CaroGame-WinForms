<<<<<<< HEAD:CaroGame-WinForms/Program.cs

using CaroGame_WinForms.Forms;
using CaroGame_WinForms.Forms.Account;

namespace CaroGame_WinForms
=======
namespace CaroGame_WinForms.Forms
>>>>>>> f24bd90a5b4dca1ae989044395c0bf9ad381ec2e:CaroGame-WinForms/Forms/Program.cs
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
<<<<<<< HEAD:CaroGame-WinForms/Program.cs
            Application.Run(new frmRegister());
=======
            //Application.Run(new Form1());
            Application.Run(new frmGame());
            //Application.Run(new frmResult(false, "PvP Online", 1000));
            //Application.Run(new frmUndoRequest());
            Application.Run(new frmMatchmaking());
>>>>>>> f24bd90a5b4dca1ae989044395c0bf9ad381ec2e:CaroGame-WinForms/Forms/Program.cs
        }
    }
}