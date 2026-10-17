namespace CaroGame.UI.Forms.Match
{
    partial class frmResult
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
            lblResultStatus = new Label();
            lblGameMode = new Label();
            lblEloChange = new Label();
            btnPlayAgain = new Button();
            btnBackToMenu = new Button();
            SuspendLayout();
            // 
            // lblResultStatus
            // 
            lblResultStatus.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblResultStatus.Location = new Point(95, 26);
            lblResultStatus.Name = "lblResultStatus";
            lblResultStatus.Size = new Size(269, 45);
            lblResultStatus.TabIndex = 0;
            lblResultStatus.Text = "Thất Bại";
            lblResultStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblGameMode
            // 
            lblGameMode.AutoSize = true;
            lblGameMode.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblGameMode.Location = new Point(46, 92);
            lblGameMode.Name = "lblGameMode";
            lblGameMode.Size = new Size(108, 21);
            lblGameMode.TabIndex = 1;
            lblGameMode.Text = "lblGameMode";
            // 
            // lblEloChange
            // 
            lblEloChange.AutoSize = true;
            lblEloChange.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEloChange.Location = new Point(46, 130);
            lblEloChange.Name = "lblEloChange";
            lblEloChange.Size = new Size(101, 21);
            lblEloChange.TabIndex = 2;
            lblEloChange.Text = "lblEloChange";
            // 
            // btnPlayAgain
            // 
            btnPlayAgain.Location = new Point(46, 165);
            btnPlayAgain.Name = "btnPlayAgain";
            btnPlayAgain.Size = new Size(111, 23);
            btnPlayAgain.TabIndex = 3;
            btnPlayAgain.Text = "Ván Mới";
            btnPlayAgain.UseVisualStyleBackColor = true;
            // 
            // btnBackToMenu
            // 
            btnBackToMenu.Location = new Point(253, 165);
            btnBackToMenu.Name = "btnBackToMenu";
            btnBackToMenu.Size = new Size(111, 23);
            btnBackToMenu.TabIndex = 4;
            btnBackToMenu.Text = "Back To Menu";
            btnBackToMenu.UseVisualStyleBackColor = true;
            // 
            // frmResult
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(412, 234);
            Controls.Add(btnBackToMenu);
            Controls.Add(btnPlayAgain);
            Controls.Add(lblEloChange);
            Controls.Add(lblGameMode);
            Controls.Add(lblResultStatus);
            Name = "frmResult";
            Text = "frmResult";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblResultStatus;
        private Label lblGameMode;
        private Label lblEloChange;
        private Button btnPlayAgain;
        private Button btnBackToMenu;
    }
}