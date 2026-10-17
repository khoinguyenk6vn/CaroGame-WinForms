namespace CaroGame.UI.Forms.Menu
{
    partial class frmProfile
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
            pictureBox1 = new PictureBox();
            lblUsername = new Label();
            lblTitle1 = new Label();
            lblTitle2 = new Label();
            lblScore = new Label();
            lblRank = new Label();
            btnChangeAvatar = new Button();
            btnHistory = new Button();
            btnLogout = new Button();
            btnLoginToSave = new Button();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Nham_Coffe;
            pictureBox1.Location = new Point(12, 41);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(123, 123);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(233, 41);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(71, 15);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Tên của Bạn";
            // 
            // lblTitle1
            // 
            lblTitle1.AutoSize = true;
            lblTitle1.Location = new Point(156, 41);
            lblTitle1.Name = "lblTitle1";
            lblTitle1.Size = new Size(45, 15);
            lblTitle1.TabIndex = 2;
            lblTitle1.Text = "Name :";
            // 
            // lblTitle2
            // 
            lblTitle2.AutoSize = true;
            lblTitle2.Location = new Point(156, 78);
            lblTitle2.Name = "lblTitle2";
            lblTitle2.Size = new Size(42, 15);
            lblTitle2.TabIndex = 3;
            lblTitle2.Text = "Score :";
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.Location = new Point(233, 78);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(83, 15);
            lblScore.TabIndex = 4;
            lblScore.Text = "Score Của Bạn";
            // 
            // lblRank
            // 
            lblRank.AutoSize = true;
            lblRank.Location = new Point(156, 116);
            lblRank.Name = "lblRank";
            lblRank.Size = new Size(80, 15);
            lblRank.TabIndex = 5;
            lblRank.Text = "Rank Của Bạn";
            // 
            // btnChangeAvatar
            // 
            btnChangeAvatar.Location = new Point(12, 193);
            btnChangeAvatar.Name = "btnChangeAvatar";
            btnChangeAvatar.Size = new Size(123, 23);
            btnChangeAvatar.TabIndex = 6;
            btnChangeAvatar.Text = "Thay Đổi Avatar";
            btnChangeAvatar.UseVisualStyleBackColor = true;
            // 
            // btnHistory
            // 
            btnHistory.Location = new Point(211, 193);
            btnHistory.Name = "btnHistory";
            btnHistory.Size = new Size(75, 23);
            btnHistory.TabIndex = 7;
            btnHistory.Text = "Lịch Sử";
            btnHistory.UseVisualStyleBackColor = true;
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(362, 193);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(75, 23);
            btnLogout.TabIndex = 8;
            btnLogout.Text = "Đăng Xuất";
            btnLogout.UseVisualStyleBackColor = true;
            // 
            // btnLoginToSave
            // 
            btnLoginToSave.Location = new Point(266, 262);
            btnLoginToSave.Name = "btnLoginToSave";
            btnLoginToSave.Size = new Size(110, 23);
            btnLoginToSave.TabIndex = 9;
            btnLoginToSave.Text = "Đăng Nhập/Lưu";
            btnLoginToSave.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(513, 193);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 10;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // frmProfile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(627, 324);
            Controls.Add(btnBack);
            Controls.Add(btnLoginToSave);
            Controls.Add(btnLogout);
            Controls.Add(btnHistory);
            Controls.Add(btnChangeAvatar);
            Controls.Add(lblRank);
            Controls.Add(lblScore);
            Controls.Add(lblTitle2);
            Controls.Add(lblTitle1);
            Controls.Add(lblUsername);
            Controls.Add(pictureBox1);
            Name = "frmProfile";
            Text = "frmProfile";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblUsername;
        private Label lblTitle1;
        private Label lblTitle2;
        private Label lblScore;
        private Label lblRank;
        private Button btnChangeAvatar;
        private Button btnHistory;
        private Button btnLogout;
        private Button btnLoginToSave;
        private Button btnBack;
    }
}