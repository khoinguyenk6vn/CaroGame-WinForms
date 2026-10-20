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
            components = new System.ComponentModel.Container();
            btnBack = new Button();
            label1 = new Label();
            txtEmail = new TextBox();
            btnSendCode = new Button();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(455, 102);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 11;
            label1.Text = "Quên mật khẩu";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(346, 157);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Email";
            txtEmail.Size = new Size(321, 23);
            txtEmail.TabIndex = 12;
            // 
            // btnSendCode
            // 
            btnSendCode.Location = new Point(449, 278);
            btnSendCode.Name = "btnSendCode";
            btnSendCode.Size = new Size(105, 23);
            btnSendCode.TabIndex = 13;
            btnSendCode.Text = "Gửi mã xác minh";
            btnSendCode.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // frmForgotPassword
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 537);
            Controls.Add(btnSendCode);
            Controls.Add(txtEmail);
            Controls.Add(label1);
            Controls.Add(btnBack);
            Name = "frmForgotPassword";
            Text = "frmForgotPassword";
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private Label label1;
        private TextBox txtEmail;
        private Button btnSendCode;
        private ErrorProvider errorProvider;
    }
}