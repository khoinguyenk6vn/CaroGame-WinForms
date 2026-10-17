namespace CaroGame.UI.Forms.Menu
{
    partial class frmInformation
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
            rtbGuide = new RichTextBox();
            btnClose = new Button();
            SuspendLayout();
            // 
            // rtbGuide
            // 
            rtbGuide.Location = new Point(2, 12);
            rtbGuide.Name = "rtbGuide";
            rtbGuide.ReadOnly = true;
            rtbGuide.Size = new Size(801, 438);
            rtbGuide.TabIndex = 0;
            rtbGuide.Text = "";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(331, 337);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 23);
            btnClose.TabIndex = 1;
            btnClose.Text = "ĐÓNG";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // frmInformation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnClose);
            Controls.Add(rtbGuide);
            Name = "frmInformation";
            Text = "frmInformation";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox rtbGuide;
        private Button btnClose;
    }
}