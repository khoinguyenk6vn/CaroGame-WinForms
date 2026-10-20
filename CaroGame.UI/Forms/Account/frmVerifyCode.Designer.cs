namespace CaroGame.UI.Forms.Account
{
    partial class frmVerifyCode
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
            lblMessage = new Label();
            lblResendCountdown = new Label();
            txtCode = new TextBox();
            btnConfirm = new Button();
            btnResend = new Button();
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
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(380, 94);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(241, 15);
            lblMessage.TabIndex = 11;
            lblMessage.Text = "Mã xác minh đã được gửi đến email của bạn";
            // 
            // lblResendCountdown
            // 
            lblResendCountdown.AutoSize = true;
            lblResendCountdown.Location = new Point(360, 188);
            lblResendCountdown.Name = "lblResendCountdown";
            lblResendCountdown.Size = new Size(138, 15);
            lblResendCountdown.TabIndex = 12;
            lblResendCountdown.Text = "Có thể gửi lại sau 60 giây";
            // 
            // txtCode
            // 
            txtCode.Location = new Point(360, 162);
            txtCode.Name = "txtCode";
            txtCode.PlaceholderText = "Code";
            txtCode.Size = new Size(216, 23);
            txtCode.TabIndex = 13;
            // 
            // btnConfirm
            // 
            btnConfirm.Location = new Point(469, 312);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(75, 23);
            btnConfirm.TabIndex = 14;
            btnConfirm.Text = "Xác nhận";
            btnConfirm.UseVisualStyleBackColor = true;
            // 
            // btnResend
            // 
            btnResend.Location = new Point(582, 162);
            btnResend.Name = "btnResend";
            btnResend.Size = new Size(75, 23);
            btnResend.TabIndex = 15;
            btnResend.Text = "Gửi lại mã";
            btnResend.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // frmVerifyCode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 537);
            Controls.Add(btnResend);
            Controls.Add(btnConfirm);
            Controls.Add(txtCode);
            Controls.Add(lblResendCountdown);
            Controls.Add(lblMessage);
            Controls.Add(btnBack);
            Name = "frmVerifyCode";
            Text = "frmVerifyCode";
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private Label lblMessage;
        private Label lblResendCountdown;
        private TextBox txtCode;
        private Button btnConfirm;
        private Button btnResend;
        private ErrorProvider errorProvider;
    }
}