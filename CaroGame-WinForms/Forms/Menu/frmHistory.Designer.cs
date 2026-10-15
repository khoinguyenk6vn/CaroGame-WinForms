namespace CaroGame_WinForms.Forms.Menu
{
    partial class frmHistory
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvHistory = new DataGridView();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            SuspendLayout();
            // 
            // dgvHistory
            // 
            dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistory.Location = new Point(1, 0);
            dgvHistory.Name = "dgvHistory";
            dgvHistory.Size = new Size(799, 375);
            dgvHistory.TabIndex = 0;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(1, 381);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(118, 57);
            btnBack.TabIndex = 1;
            btnBack.Text = "GO BACK BỜ RÔ";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // frmHistory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnBack);
            Controls.Add(dgvHistory);
            Name = "frmHistory";
            Text = "frmHistory";
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvHistory;
        private Button btnBack;
    }
}