namespace CaroGame_WinForms.Forms.Account
{
    partial class frmResetPassword
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
            txtNewPassword = new TextBox();
            txtConfirmPassword = new TextBox();
            btnConfirm = new Button();
            lblError = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Location = new Point(12, 12);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 10;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // txtNewPassword
            // 
            txtNewPassword.Location = new Point(476, 233);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.Size = new Size(322, 23);
            txtNewPassword.TabIndex = 11;
            txtNewPassword.Text = "Ô nhập mật khẩu mới";
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Location = new Point(476, 301);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(322, 23);
            txtConfirmPassword.TabIndex = 12;
            txtConfirmPassword.Text = "Ô xác nhận mật khẩu";
            // 
            // btnConfirm
            // 
            btnConfirm.Location = new Point(591, 402);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(75, 23);
            btnConfirm.TabIndex = 14;
            btnConfirm.Text = "Đổi mật khẩu";
            btnConfirm.UseVisualStyleBackColor = true;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Location = new Point(935, 304);
            lblError.Name = "lblError";
            lblError.Size = new Size(98, 15);
            lblError.TabIndex = 17;
            lblError.Text = "Cảnh báo lỗi (ẩn)";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(591, 172);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 18;
            label1.Text = "Đổi mật khẩu";
            // 
            // frmResetPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1278, 761);
            Controls.Add(label1);
            Controls.Add(lblError);
            Controls.Add(btnConfirm);
            Controls.Add(txtConfirmPassword);
            Controls.Add(txtNewPassword);
            Controls.Add(btnBack);
            Name = "frmResetPassword";
            Text = "frmResetPassword";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnConfirm;
        private Label lblError;
        private Label label1;
    }
}