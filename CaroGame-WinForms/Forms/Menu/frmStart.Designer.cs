namespace CaroGame_WinForms.Forms.Menu
{
    partial class frmStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStart));
            btnPlayNow = new Button();
            btnLogin = new Button();
            toolStripSettings = new ToolStrip();
            toolStripButton1 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            lblTitle = new Label();
            toolStripSettings.SuspendLayout();
            SuspendLayout();
            // 
            // btnPlayNow
            // 
            btnPlayNow.Location = new Point(356, 140);
            btnPlayNow.Name = "btnPlayNow";
            btnPlayNow.Size = new Size(75, 23);
            btnPlayNow.TabIndex = 0;
            btnPlayNow.Text = "Chơi ngay";
            btnPlayNow.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(356, 181);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 23);
            btnLogin.TabIndex = 1;
            btnLogin.Text = "Đăng nhập";
            btnLogin.UseVisualStyleBackColor = true;
            // 
            // toolStripSettings
            // 
            toolStripSettings.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripButton2 });
            toolStripSettings.Location = new Point(0, 0);
            toolStripSettings.Name = "toolStripSettings";
            toolStripSettings.Size = new Size(800, 25);
            toolStripSettings.TabIndex = 2;
            toolStripSettings.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(23, 22);
            toolStripButton2.Text = "toolStripButton2";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(318, 38);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(141, 15);
            lblTitle.TabIndex = 3;
            lblTitle.Text = "CARO GAMESSSSSSSSSSS";
            // 
            // frmStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblTitle);
            Controls.Add(toolStripSettings);
            Controls.Add(btnLogin);
            Controls.Add(btnPlayNow);
            Name = "frmStart";
            Text = "frmStart";
            toolStripSettings.ResumeLayout(false);
            toolStripSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnPlayNow;
        private Button btnLogin;
        private ToolStrip toolStripSettings;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private Label lblTitle;
    }
}