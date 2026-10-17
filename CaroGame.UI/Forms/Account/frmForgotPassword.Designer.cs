namespace CaroGame.UI.Forms.Account
{
    partial class frmForgotPassword
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
            btnBack = new Button();
            btnContinue = new Button();
            label1 = new Label();
            txtEmail = new TextBox();
            btnSendCode = new Button();
            lblError = new Label();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Location = new Point(12, 12);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 9;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // btnContinue
            // 
            btnContinue.Location = new Point(595, 478);
            btnContinue.Name = "btnContinue";
            btnContinue.Size = new Size(0, 0);
            btnContinue.TabIndex = 10;
            btnContinue.Text = "Tiếp tục";
            btnContinue.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(581, 148);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 11;
            label1.Text = "Quên mật khẩu";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(465, 217);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(321, 23);
            txtEmail.TabIndex = 12;
            txtEmail.Text = "Email";
            // 
            // btnSendCode
            // 
            btnSendCode.Location = new Point(565, 380);
            btnSendCode.Name = "btnSendCode";
            btnSendCode.Size = new Size(105, 23);
            btnSendCode.TabIndex = 13;
            btnSendCode.Text = "Gửi mã xác minh";
            btnSendCode.UseVisualStyleBackColor = true;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Location = new Point(957, 304);
            lblError.Name = "lblError";
            lblError.Size = new Size(98, 15);
            lblError.TabIndex = 14;
            lblError.Text = "Cảnh báo lỗi (ẩn)";
            // 
            // frmForgotPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1278, 761);
            Controls.Add(lblError);
            Controls.Add(btnSendCode);
            Controls.Add(txtEmail);
            Controls.Add(label1);
            Controls.Add(btnContinue);
            Controls.Add(btnBack);
            Name = "frmForgotPassword";
            Text = "frmForgotPassword";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private Button btnContinue;
        private Label label1;
        private TextBox txtEmail;
        private Button btnSendCode;
        private Label lblError;
    }
}