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
            components = new System.ComponentModel.Container();
            lblMessage = new Label();
            btnAccept = new Button();
            btnDecline = new Button();
            lblCountdown = new Label();
            tmrCountdown = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMessage.ForeColor = Color.Red;
            lblMessage.Location = new Point(97, 93);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(521, 21);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "Đối thủ muốn xin đi lại (Undo) nước vừa rồi. Bạn có đồng ý không?";
            // 
            // btnAccept
            // 
            btnAccept.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAccept.Location = new Point(211, 210);
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
            btnDecline.Location = new Point(395, 210);
            btnDecline.Name = "btnDecline";
            btnDecline.Size = new Size(90, 43);
            btnDecline.TabIndex = 2;
            btnDecline.Text = "Từ Chối";
            btnDecline.UseVisualStyleBackColor = true;
            //
            // lblCountdown
            //
            lblCountdown.AutoSize = true;
            lblCountdown.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCountdown.Location = new Point(97, 140);
            lblCountdown.Name = "lblCountdown";
            lblCountdown.Size = new Size(105, 21);
            lblCountdown.TabIndex = 3;
            lblCountdown.Text = "lblCountdown";
            //
            // tmrCountdown
            //
            tmrCountdown.Interval = 1000;
            //
            // frmUndoRequest
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(704, 366);
            Controls.Add(lblCountdown);
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
        private Label lblCountdown;
        private System.Windows.Forms.Timer tmrCountdown;
    }
}