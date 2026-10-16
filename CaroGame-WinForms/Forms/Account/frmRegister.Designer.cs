namespace CaroGame_WinForms.Forms.Account
{
    partial class frmRegister
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
            label1 = new Label();
            txtUsername = new TextBox();
            txtEmail = new TextBox();
            txtPassword = new TextBox();
            txtConfirmPassword = new TextBox();
            btnContinue = new Button();
            lnkLogin = new LinkLabel();
            btnBack = new Button();
            lblError = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(590, 146);
            label1.Name = "label1";
            label1.Size = new Size(50, 15);
            label1.TabIndex = 0;
            label1.Text = "Đăng ký";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(452, 212);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(321, 23);
            txtUsername.TabIndex = 1;
            txtUsername.Text = "Username";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(452, 271);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(321, 23);
            txtEmail.TabIndex = 2;
            txtEmail.Text = "Email";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(452, 332);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(321, 23);
            txtPassword.TabIndex = 3;
            txtPassword.Text = "Password";
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Location = new Point(452, 397);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(321, 23);
            txtConfirmPassword.TabIndex = 4;
            txtConfirmPassword.Text = "ConfirmPassword";
            txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // btnContinue
            // 
            btnContinue.Location = new Point(579, 552);
            btnContinue.Name = "btnContinue";
            btnContinue.Size = new Size(75, 23);
            btnContinue.TabIndex = 5;
            btnContinue.Text = "Tiếp tục";
            btnContinue.UseVisualStyleBackColor = true;
            // 
            // lnkLogin
            // 
            lnkLogin.AutoSize = true;
            lnkLogin.Location = new Point(618, 435);
            lnkLogin.Name = "lnkLogin";
            lnkLogin.Size = new Size(155, 15);
            lnkLogin.TabIndex = 6;
            lnkLogin.TabStop = true;
            lnkLogin.Text = "Đã có tài khoản? Đăng nhập";
            // 
            // btnBack
            // 
            btnBack.Location = new Point(12, 12);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(75, 23);
            btnBack.TabIndex = 7;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Location = new Point(972, 303);
            lblError.Name = "lblError";
            lblError.Size = new Size(98, 15);
            lblError.TabIndex = 8;
            lblError.Text = "Cảnh báo lỗi (ẩn)";
            // 
            // frmRegister
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1278, 761);
            Controls.Add(lblError);
            Controls.Add(btnBack);
            Controls.Add(lnkLogin);
            Controls.Add(btnContinue);
            Controls.Add(txtConfirmPassword);
            Controls.Add(txtPassword);
            Controls.Add(txtEmail);
            Controls.Add(txtUsername);
            Controls.Add(label1);
            Name = "frmRegister";
            Text = "frmRegister";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnContinue;
        private LinkLabel lnkLogin;
        private Button btnBack;
        private Label lblError;
    }
}