using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CaroGame_WinForms.Forms.Menu
{
    public partial class frmInformation : Form
    {
        public frmInformation()
        {
            InitializeComponent();

            btnClose.Click += btnClose_Click;
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
