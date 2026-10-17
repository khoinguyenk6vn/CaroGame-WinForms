namespace CaroGame.UI.Forms.Account
{
    partial class frmLogin
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
            txtPassword = new TextBox();
            btnContinue = new Button();
            lnkRegister = new LinkLabel();
            btnBack = new Button();
            lblError = new Label();
            lnkForgotPassword = new LinkLabel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(666, 197);
            label1.Name = "label1";
            label1.Size = new Size(82, 20);
            label1.TabIndex = 0;
            label1.Text = "Đăng nhập";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(517, 297);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(366, 27);
            txtUsername.TabIndex = 1;
            txtUsername.Text = "Username";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(517, 385);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(366, 27);
            txtPassword.TabIndex = 2;
            txtPassword.Text = "Password";
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnContinue
            // 
            btnContinue.Location = new Point(666, 663);
            btnContinue.Margin = new Padding(3, 4, 3, 4);
            btnContinue.Name = "btnContinue";
            btnContinue.Size = new Size(86, 31);
            btnContinue.TabIndex = 6;
            btnContinue.Text = "Tiếp tục";
            btnContinue.UseVisualStyleBackColor = true;
            // 
            // lnkRegister
            // 
            lnkRegister.AutoSize = true;
            lnkRegister.Location = new Point(517, 435);
            lnkRegister.Name = "lnkRegister";
            lnkRegister.Size = new Size(99, 20);
            lnkRegister.TabIndex = 7;
            lnkRegister.TabStop = true;
            lnkRegister.Text = "Đăng ký ngay";
            // 
            // btnBack
            // 
            btnBack.Location = new Point(14, 16);
            btnBack.Margin = new Padding(3, 4, 3, 4);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(86, 31);
            btnBack.TabIndex = 8;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.Location = new Point(1110, 385);
            lblError.Name = "lblError";
            lblError.Size = new Size(123, 20);
            lblError.TabIndex = 9;
            lblError.Text = "Cảnh báo lỗi (ẩn)";
            // 
            // lnkForgotPassword
            // 
            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Location = new Point(782, 435);
            lnkForgotPassword.Name = "lnkForgotPassword";
            lnkForgotPassword.Size = new Size(109, 20);
            lnkForgotPassword.TabIndex = 10;
            lnkForgotPassword.TabStop = true;
            lnkForgotPassword.Text = "Quên mật khẩu";
            // 
            // frmLogin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1410, 717);
            Controls.Add(lnkForgotPassword);
            Controls.Add(lblError);
            Controls.Add(btnBack);
            Controls.Add(lnkRegister);
            Controls.Add(btnContinue);
            Controls.Add(txtPassword);
            Controls.Add(txtUsername);
            Controls.Add(label1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "frmLogin";
            Text = "frmLogin";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnContinue;
        private LinkLabel lnkRegister;
        private Button btnBack;
        private Label lblError;
        private LinkLabel lnkForgotPassword;
    }
}