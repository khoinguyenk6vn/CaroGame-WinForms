namespace CaroGame_WinForms.Forms.Menu
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
            chkSound = new CheckBox();
            chkMusic = new CheckBox();
            btnClose = new Button();
            SuspendLayout();
            // 
            // chkSound
            // 
            chkSound.AutoSize = true;
            chkSound.Location = new Point(77, 37);
            chkSound.Name = "chkSound";
            chkSound.Size = new Size(60, 19);
            chkSound.TabIndex = 0;
            chkSound.Text = "Sound";
            chkSound.UseVisualStyleBackColor = true;
            // 
            // chkMusic
            // 
            chkMusic.AutoSize = true;
            chkMusic.Location = new Point(77, 81);
            chkMusic.Name = "chkMusic";
            chkMusic.Size = new Size(58, 19);
            chkMusic.TabIndex = 1;
            chkMusic.Text = "Music";
            chkMusic.UseVisualStyleBackColor = true;
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
            Controls.Add(chkMusic);
            Controls.Add(chkSound);
            Name = "frmSettings";
            Text = "frmSettings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkSound;
        private CheckBox chkMusic;
        private Button btnClose;
    }
}