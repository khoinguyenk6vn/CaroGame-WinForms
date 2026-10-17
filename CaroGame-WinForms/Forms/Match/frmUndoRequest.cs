namespace CaroGame_WinForms
{
    // Hộp thoại hỏi có đồng ý cho đối thủ đi lại không.
    // Trả về DialogResult.Yes (đồng ý), DialogResult.No (từ chối),
    // hoặc DialogResult.Cancel nếu hết 15 giây và frmGame tự đóng hộp thoại.
    public partial class frmUndoRequest : Form
    {
        public frmUndoRequest()
        {
            InitializeComponent();

            // Gắn các sự kiện chưa được gắn trong Designer
            btnAccept.Click += btnAccept_Click;
            btnDecline.Click += btnDecline_Click;
        }

        private void btnAccept_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnDecline_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
    }
}
