using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CaroGame_WinForms.Services;

namespace CaroGame_WinForms.Forms.Menu
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();

            // Hiện trạng thái đang lưu
            chkSound.Checked = SoundSettings.SoundOn;
            chkMusic.Checked = SoundSettings.MusicOn;

            // Gắn các sự kiện chưa được gắn trong Designer
            chkSound.CheckedChanged += chkSound_CheckedChanged;
            chkMusic.CheckedChanged += chkMusic_CheckedChanged;
            btnClose.Click += btnClose_Click;
        }

        private void chkSound_CheckedChanged(object? sender, EventArgs e)
        {
            SoundSettings.SoundOn = chkSound.Checked;
        }

        private void chkMusic_CheckedChanged(object? sender, EventArgs e)
        {
            SoundSettings.MusicOn = chkMusic.Checked;
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
