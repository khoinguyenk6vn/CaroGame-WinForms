using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CaroGame_WinForms
{
    public partial class frmUndoRequest : Form
    {
        public frmUndoRequest()
        {
            InitializeComponent();
        }

        // Nút Đồng ý: Trả về kết quả DialogResult.Yes
        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        // Nút Từ chối: Trả về kết quả DialogResult.No
        private void btnDecline_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
