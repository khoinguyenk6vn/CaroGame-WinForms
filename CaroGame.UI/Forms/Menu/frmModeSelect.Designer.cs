namespace CaroGame.UI.Forms.Menu
{
    partial class frmModeSelect
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
            btnPvE = new Button();
            btnPvPOnline = new Button();
            btnPvPOffline = new Button();
            picAvatar = new PictureBox();
            btnLeaderboard = new Button();
            lblAvatar = new Label();
            lblLeaderBoard = new Label();
            lblTitle = new Label();
            ((System.ComponentModel.ISupportInitialize)picAvatar).BeginInit();
            SuspendLayout();
            // 
            // btnPvE
            // 
            btnPvE.Location = new Point(437, 221);
            btnPvE.Name = "btnPvE";
            btnPvE.Size = new Size(103, 23);
            btnPvE.TabIndex = 0;
            btnPvE.Text = "P V E";
            btnPvE.UseVisualStyleBackColor = true;
            // 
            // btnPvPOnline
            // 
            btnPvPOnline.Location = new Point(437, 301);
            btnPvPOnline.Name = "btnPvPOnline";
            btnPvPOnline.Size = new Size(103, 23);
            btnPvPOnline.TabIndex = 1;
            btnPvPOnline.Text = "P v P Online";
            btnPvPOnline.UseVisualStyleBackColor = true;
            // 
            // btnPvPOffline
            // 
            btnPvPOffline.Location = new Point(437, 260);
            btnPvPOffline.Name = "btnPvPOffline";
            btnPvPOffline.Size = new Size(103, 23);
            btnPvPOffline.TabIndex = 2;
            btnPvPOffline.Text = "P v P Offline";
            btnPvPOffline.UseVisualStyleBackColor = true;
            // 
            // picAvatar
            // 
            picAvatar.Image = Properties.Resources.Nham_Coffe;
            picAvatar.Location = new Point(39, 157);
            picAvatar.Name = "picAvatar";
            picAvatar.Size = new Size(100, 100);
            picAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picAvatar.TabIndex = 3;
            picAvatar.TabStop = false;
            // 
            // btnLeaderboard
            // 
            btnLeaderboard.Location = new Point(915, 27);
            btnLeaderboard.Name = "btnLeaderboard";
            btnLeaderboard.Size = new Size(75, 23);
            btnLeaderboard.TabIndex = 4;
            btnLeaderboard.Text = "🏆Rankings";
            btnLeaderboard.UseVisualStyleBackColor = true;
            // 
            // lblAvatar
            // 
            lblAvatar.AutoSize = true;
            lblAvatar.Location = new Point(39, 111);
            lblAvatar.Name = "lblAvatar";
            lblAvatar.Size = new Size(86, 15);
            lblAvatar.TabIndex = 5;
            lblAvatar.Text = "Avatar của Bạn";
            // 
            // lblLeaderBoard
            // 
            lblLeaderBoard.AutoSize = true;
            lblLeaderBoard.Location = new Point(911, 9);
            lblLeaderBoard.Name = "lblLeaderBoard";
            lblLeaderBoard.Size = new Size(85, 15);
            lblLeaderBoard.TabIndex = 6;
            lblLeaderBoard.Text = "Bảng xếp hạng";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(328, 94);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(320, 32);
            lblTitle.TabIndex = 7;
            lblTitle.Text = "CHẾ ĐỘ CHƠI NHA BỜ RÔ ";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmModeSelect
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 537);
            Controls.Add(lblTitle);
            Controls.Add(lblLeaderBoard);
            Controls.Add(lblAvatar);
            Controls.Add(btnLeaderboard);
            Controls.Add(picAvatar);
            Controls.Add(btnPvPOffline);
            Controls.Add(btnPvPOnline);
            Controls.Add(btnPvE);
            Name = "frmModeSelect";
            Text = "frmModeSelect";
            ((System.ComponentModel.ISupportInitialize)picAvatar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnPvE;
        private Button btnPvPOnline;
        private Button btnPvPOffline;
        private PictureBox picAvatar;
        private Button btnLeaderboard;
        private Label lblAvatar;
        private Label lblLeaderBoard;
        private Label lblTitle;
    }
}