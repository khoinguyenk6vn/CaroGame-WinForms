namespace CaroGame.UI.Forms.Match
{
    partial class frmUndoRequest
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
            lblMessage = new Label();
            btnAccept = new Button();
            btnDecline = new Button();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMessage.ForeColor = Color.Red;
            lblMessage.Location = new Point(16, 52);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(521, 21);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "Đối thủ muốn xin đi lại (Undo) nước vừa rồi. Bạn có đồng ý không?";
            // 
            // btnAccept
            // 
            btnAccept.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAccept.Location = new Point(93, 147);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(90, 43);
            btnAccept.TabIndex = 1;
            btnAccept.Text = "Đồng Ý";
            btnAccept.UseVisualStyleBackColor = true;
            // 
            // btnDecline
            // 
            btnDecline.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDecline.ForeColor = Color.Red;
            btnDecline.Location = new Point(333, 147);
            btnDecline.Name = "btnDecline";
            btnDecline.Size = new Size(90, 43);
            btnDecline.TabIndex = 2;
            btnDecline.Text = "Từ Chối";
            btnDecline.UseVisualStyleBackColor = true;
            // 
            // frmUndoRequest
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(549, 251);
            Controls.Add(btnDecline);
            Controls.Add(btnAccept);
            Controls.Add(lblMessage);
            Name = "frmUndoRequest";
            Text = "frmUndoRequest";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMessage;
        private Button btnAccept;
        private Button btnDecline;
    }
}