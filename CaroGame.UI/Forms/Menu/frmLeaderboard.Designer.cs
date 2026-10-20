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
            components = new System.ComponentModel.Container();
            dgvLeaderboard = new DataGridView();
            colRankIcon = new DataGridViewImageColumn();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvLeaderboard).BeginInit();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // dgvLeaderboard
            // 
            dgvLeaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLeaderboard.Columns.AddRange(new DataGridViewColumn[] { colRankIcon });
            dgvLeaderboard.Location = new Point(12, 41);
            dgvLeaderboard.Name = "dgvLeaderboard";
            dgvLeaderboard.RowTemplate.Height = 40;
            dgvLeaderboard.Size = new Size(694, 484);
            dgvLeaderboard.TabIndex = 0;
            //
            // colRankIcon
            //
            colRankIcon.HeaderText = "Logo";
            colRankIcon.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colRankIcon.Name = "colRankIcon";
            colRankIcon.Width = 50;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(12, 12);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 1;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
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
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvLeaderboard;
        private DataGridViewImageColumn colRankIcon;
        private Button btnBack;
        private ErrorProvider errorProvider;
    }
}