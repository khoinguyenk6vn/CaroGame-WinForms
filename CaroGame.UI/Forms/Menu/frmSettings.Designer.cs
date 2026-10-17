namespace CaroGame.UI.Forms.Menu
{
    partial class frmSettings
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
            btnSound = new Button();
            btnMusic = new Button();
            btnClose = new Button();
            SuspendLayout();
            //
            // btnSound
            //
            btnSound.Location = new Point(77, 37);
            btnSound.Name = "btnSound";
            btnSound.Size = new Size(150, 23);
            btnSound.TabIndex = 0;
            btnSound.Text = "Âm thanh: Bật";
            btnSound.UseVisualStyleBackColor = true;
            //
            // btnMusic
            //
            btnMusic.Location = new Point(77, 81);
            btnMusic.Name = "btnMusic";
            btnMusic.Size = new Size(150, 23);
            btnMusic.TabIndex = 1;
            btnMusic.Text = "Nhạc nền: Bật";
            btnMusic.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(169, 148);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 2;
            btnClose.Text = "ĐÓNG";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(468, 213);
            Controls.Add(btnClose);
            Controls.Add(btnMusic);
            Controls.Add(btnSound);
            Name = "frmSettings";
            Text = "frmSettings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSound;
        private Button btnMusic;
        private Button btnClose;
    }
}