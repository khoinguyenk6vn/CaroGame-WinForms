namespace CaroGame.UI.Forms.Menu
{
    partial class frmLeaderboard
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
            dgvLeaderboard = new DataGridView();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvLeaderboard).BeginInit();
            SuspendLayout();
            // 
            // dgvLeaderboard
            // 
            dgvLeaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLeaderboard.Location = new Point(0, 0);
            dgvLeaderboard.Name = "dgvLeaderboard";
            dgvLeaderboard.Size = new Size(800, 368);
            dgvLeaderboard.TabIndex = 0;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(0, 383);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(166, 65);
            btnBack.TabIndex = 1;
            btnBack.Text = "GO BACK BỜ RÔ";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // frmLeaderboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 537);
            Controls.Add(btnBack);
            Controls.Add(dgvLeaderboard);
            Name = "frmLeaderboard";
            Text = "frmLeaderboard";
            ((System.ComponentModel.ISupportInitialize)dgvLeaderboard).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvLeaderboard;
        private Button btnBack;
    }
}