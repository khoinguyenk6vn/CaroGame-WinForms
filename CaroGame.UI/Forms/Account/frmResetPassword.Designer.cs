namespace CaroGame.UI.Forms.Account
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
            components = new System.ComponentModel.Container();
            btnBack = new Button();
            txtNewPassword = new TextBox();
            txtConfirmPassword = new TextBox();
            btnConfirm = new Button();
            label1 = new Label();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
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
            txtNewPassword.Location = new Point(343, 157);
            txtNewPassword.Name = "txtNewPassword";
            txtNewPassword.PlaceholderText = "Mật khẩu mới";
            txtNewPassword.Size = new Size(322, 23);
            txtNewPassword.TabIndex = 11;
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Location = new Point(343, 225);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.PlaceholderText = "Xác nhận mật khẩu";
            txtConfirmPassword.Size = new Size(322, 23);
            txtConfirmPassword.TabIndex = 12;
            // 
            // btnConfirm
            // 
            btnConfirm.Location = new Point(467, 322);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(75, 23);
            btnConfirm.TabIndex = 14;
            btnConfirm.Text = "Đổi mật khẩu";
            btnConfirm.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(464, 96);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 18;
            label1.Text = "Đổi mật khẩu";
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // frmResetPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 537);
            Controls.Add(label1);
            Controls.Add(btnConfirm);
            Controls.Add(txtConfirmPassword);
            Controls.Add(txtNewPassword);
            Controls.Add(btnBack);
            Name = "frmResetPassword";
            Text = "frmResetPassword";
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnConfirm;
        private Label label1;
        private ErrorProvider errorProvider;
    }
}