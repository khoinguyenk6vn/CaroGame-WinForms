namespace CaroGame.UI.Forms.Match
{
    partial class frmMatchmaking
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
            lblStatus = new Label();
            lblTimer = new Label();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(243, 80);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(193, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "\"Đang tìm kiếm đối thủ phù hợp...\"";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Location = new Point(290, 120);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(95, 15);
            lblTimer.TabIndex = 1;
            lblTimer.Text = "Thời gian tìm: 0s";
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(276, 196);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(125, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Dừng Tìm Trận";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // frmMatchmaking
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(704, 366);
            Controls.Add(btnCancel);
            Controls.Add(lblTimer);
            Controls.Add(lblStatus);
            Name = "frmMatchmaking";
            Text = "frmMatchmaking";
            Load += frmMatchmaking_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblStatus;
        private Label lblTimer;
        private Button btnCancel;
    }
}