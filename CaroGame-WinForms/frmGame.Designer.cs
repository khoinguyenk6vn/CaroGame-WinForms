namespace CaroGame_WinForms
{
    partial class frmGame
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
            pnlBoard = new Panel();
            picPlayer1Avatar = new PictureBox();
            lblPlayer1Name = new Label();
            btnGameSettings = new Button();
            pnlGameSettings = new Panel();
            btnSurrender = new Button();
            btnUndo = new Button();
            pnlPlayer1 = new Panel();
            lblPlayer1Timer = new Label();
            lblTurn = new Label();
            pnlPlayer2 = new Panel();
            lblPlayer2Timer = new Label();
            lblPlayer2Name = new Label();
            picPlayer2Avatar = new PictureBox();
            pnlChatBox = new Panel();
            btnSendChat = new Button();
            txtChatInput = new TextBox();
            listBoxChat = new ListBox();
            lblConnectionStatus = new Label();
            lblRule = new Label();
            Player1Timer = new System.Windows.Forms.Timer(components);
            Player2Timer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)picPlayer1Avatar).BeginInit();
            pnlGameSettings.SuspendLayout();
            pnlPlayer1.SuspendLayout();
            pnlPlayer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayer2Avatar).BeginInit();
            pnlChatBox.SuspendLayout();
            SuspendLayout();
            // 
            // pnlBoard
            // 
            pnlBoard.Location = new Point(1, 28);
            pnlBoard.Name = "pnlBoard";
            pnlBoard.Size = new Size(606, 600);
            pnlBoard.TabIndex = 0;
            pnlBoard.Paint += pnlBoard_Paint;
            // 
            // picPlayer1Avatar
            // 
            picPlayer1Avatar.Image = Properties.Resources.Nham_Coffe;
            picPlayer1Avatar.Location = new Point(3, 3);
            picPlayer1Avatar.Name = "picPlayer1Avatar";
            picPlayer1Avatar.Size = new Size(150, 150);
            picPlayer1Avatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picPlayer1Avatar.TabIndex = 3;
            picPlayer1Avatar.TabStop = false;
            // 
            // lblPlayer1Name
            // 
            lblPlayer1Name.Location = new Point(159, 13);
            lblPlayer1Name.Name = "lblPlayer1Name";
            lblPlayer1Name.Size = new Size(223, 30);
            lblPlayer1Name.TabIndex = 4;
            lblPlayer1Name.Text = "Tên Người Chơi 1";
            // 
            // btnGameSettings
            // 
            btnGameSettings.Location = new Point(1, -1);
            btnGameSettings.Name = "btnGameSettings";
            btnGameSettings.Size = new Size(72, 23);
            btnGameSettings.TabIndex = 1;
            btnGameSettings.Text = "Cài Đặt";
            btnGameSettings.UseVisualStyleBackColor = true;
            btnGameSettings.Click += btnGameSettings_Click;
            // 
            // pnlGameSettings
            // 
            pnlGameSettings.Controls.Add(btnSurrender);
            pnlGameSettings.Controls.Add(btnUndo);
            pnlGameSettings.Location = new Point(73, -1);
            pnlGameSettings.Name = "pnlGameSettings";
            pnlGameSettings.Size = new Size(75, 57);
            pnlGameSettings.TabIndex = 2;
            pnlGameSettings.Visible = false;
            // 
            // btnSurrender
            // 
            btnSurrender.Location = new Point(0, 29);
            btnSurrender.Name = "btnSurrender";
            btnSurrender.Size = new Size(75, 23);
            btnSurrender.TabIndex = 1;
            btnSurrender.Text = "FF";
            btnSurrender.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            btnUndo.Location = new Point(0, 0);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(75, 23);
            btnUndo.TabIndex = 0;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            // 
            // pnlPlayer1
            // 
            pnlPlayer1.Controls.Add(lblPlayer1Timer);
            pnlPlayer1.Controls.Add(lblPlayer1Name);
            pnlPlayer1.Controls.Add(picPlayer1Avatar);
            pnlPlayer1.Location = new Point(649, 28);
            pnlPlayer1.Name = "pnlPlayer1";
            pnlPlayer1.Size = new Size(400, 150);
            pnlPlayer1.TabIndex = 5;
            // 
            // lblPlayer1Timer
            // 
            lblPlayer1Timer.Location = new Point(159, 43);
            lblPlayer1Timer.Name = "lblPlayer1Timer";
            lblPlayer1Timer.Size = new Size(223, 30);
            lblPlayer1Timer.TabIndex = 5;
            lblPlayer1Timer.Text = "Timer 1";
            // 
            // lblTurn
            // 
            lblTurn.Location = new Point(3, 0);
            lblTurn.Name = "lblTurn";
            lblTurn.Size = new Size(107, 28);
            lblTurn.TabIndex = 0;
            lblTurn.Text = "label1";
            // 
            // pnlPlayer2
            // 
            pnlPlayer2.Controls.Add(lblPlayer2Timer);
            pnlPlayer2.Controls.Add(lblPlayer2Name);
            pnlPlayer2.Controls.Add(picPlayer2Avatar);
            pnlPlayer2.Location = new Point(649, 201);
            pnlPlayer2.Name = "pnlPlayer2";
            pnlPlayer2.Size = new Size(400, 150);
            pnlPlayer2.TabIndex = 6;
            // 
            // lblPlayer2Timer
            // 
            lblPlayer2Timer.Location = new Point(159, 58);
            lblPlayer2Timer.Name = "lblPlayer2Timer";
            lblPlayer2Timer.Size = new Size(223, 30);
            lblPlayer2Timer.TabIndex = 7;
            lblPlayer2Timer.Text = "Timer 2";
            // 
            // lblPlayer2Name
            // 
            lblPlayer2Name.Location = new Point(159, 28);
            lblPlayer2Name.Name = "lblPlayer2Name";
            lblPlayer2Name.Size = new Size(223, 30);
            lblPlayer2Name.TabIndex = 6;
            lblPlayer2Name.Text = "Tên Người Chơi 2";
            // 
            // picPlayer2Avatar
            // 
            picPlayer2Avatar.Image = Properties.Resources.Nham_Coffe;
            picPlayer2Avatar.Location = new Point(3, 0);
            picPlayer2Avatar.Name = "picPlayer2Avatar";
            picPlayer2Avatar.Size = new Size(150, 150);
            picPlayer2Avatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picPlayer2Avatar.TabIndex = 6;
            picPlayer2Avatar.TabStop = false;
            // 
            // pnlChatBox
            // 
            pnlChatBox.Controls.Add(btnSendChat);
            pnlChatBox.Controls.Add(txtChatInput);
            pnlChatBox.Controls.Add(listBoxChat);
            pnlChatBox.Controls.Add(lblConnectionStatus);
            pnlChatBox.Controls.Add(lblRule);
            pnlChatBox.Controls.Add(lblTurn);
            pnlChatBox.Location = new Point(649, 374);
            pnlChatBox.Name = "pnlChatBox";
            pnlChatBox.Size = new Size(400, 238);
            pnlChatBox.TabIndex = 6;
            // 
            // btnSendChat
            // 
            btnSendChat.Location = new Point(342, 196);
            btnSendChat.Name = "btnSendChat";
            btnSendChat.Size = new Size(55, 23);
            btnSendChat.TabIndex = 5;
            btnSendChat.Text = "Gửi";
            btnSendChat.UseVisualStyleBackColor = true;
            btnSendChat.Click += btnSendChat_Click;
            // 
            // txtChatInput
            // 
            txtChatInput.Location = new Point(3, 197);
            txtChatInput.Name = "txtChatInput";
            txtChatInput.Size = new Size(333, 23);
            txtChatInput.TabIndex = 4;
            // 
            // listBoxChat
            // 
            listBoxChat.FormattingEnabled = true;
            listBoxChat.Location = new Point(3, 31);
            listBoxChat.Name = "listBoxChat";
            listBoxChat.Size = new Size(394, 199);
            listBoxChat.TabIndex = 3;
            // 
            // lblConnectionStatus
            // 
            lblConnectionStatus.Location = new Point(229, 0);
            lblConnectionStatus.Name = "lblConnectionStatus";
            lblConnectionStatus.Size = new Size(107, 28);
            lblConnectionStatus.TabIndex = 2;
            lblConnectionStatus.Text = "label1";
            // 
            // lblRule
            // 
            lblRule.Location = new Point(116, 0);
            lblRule.Name = "lblRule";
            lblRule.Size = new Size(107, 28);
            lblRule.TabIndex = 1;
            lblRule.Text = "label1";
            // 
            // Player1Timer
            // 
            Player1Timer.Enabled = true;
            Player1Timer.Interval = 1000;
            // 
            // Player2Timer
            // 
            Player2Timer.Enabled = true;
            Player2Timer.Interval = 1000;
            // 
            // frmGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1168, 627);
            Controls.Add(pnlChatBox);
            Controls.Add(pnlPlayer2);
            Controls.Add(pnlPlayer1);
            Controls.Add(pnlGameSettings);
            Controls.Add(btnGameSettings);
            Controls.Add(pnlBoard);
            Name = "frmGame";
            Text = "frmGame";
            Load += frmGame_Load;
            ((System.ComponentModel.ISupportInitialize)picPlayer1Avatar).EndInit();
            pnlGameSettings.ResumeLayout(false);
            pnlPlayer1.ResumeLayout(false);
            pnlPlayer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPlayer2Avatar).EndInit();
            pnlChatBox.ResumeLayout(false);
            pnlChatBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlBoard;
        private Button btnGameSettings;
        private Panel pnlGameSettings;
        private Button btnSurrender;
        private Button btnUndo;
        private PictureBox picPlayer1Avatar;
        private Label lblPlayer1Name;
        private Panel pnlPlayer1;
        private Panel pnlPlayer2;
        private Panel pnlChatBox;
        private Label lblPlayer1Timer;
        private System.Windows.Forms.Timer Player1Timer;
        private Label lblPlayer2Timer;
        private Label lblPlayer2Name;
        private PictureBox picPlayer2Avatar;
        private System.Windows.Forms.Timer Player2Timer;
        private Label lblTurn;
        private Label lblRule;
        private ListBox listBoxChat;
        private Label lblConnectionStatus;
        private Button btnSendChat;
        private TextBox txtChatInput;
    }
}