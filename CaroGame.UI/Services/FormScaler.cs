namespace CaroGame.UI.Services
{
    // Co giãn toàn bộ control theo kích thước cửa sổ, để khi phóng to (Maximize) giao diện tràn hết màn hình.
    // Cách làm: lúc Form vừa hiện lên, ghi lại vị trí, kích thước, cỡ chữ ban đầu của từng control;
    // mỗi lần cửa sổ đổi kích thước thì nhân tất cả theo tỉ lệ (kích thước mới / kích thước ban đầu).
    // Dùng: thêm dòng  new FormScaler(this);  vào constructor của Form, sau InitializeComponent().
    public class FormScaler
    {
        private readonly Form form;
        private readonly Dictionary<Control, Rectangle> originalBounds = new Dictionary<Control, Rectangle>();
        private readonly Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();
        private Size originalClientSize;

        public FormScaler(Form form)
        {
            this.form = form;
            form.Shown += Form_Shown;
            form.Resize += Form_Resize;
        }

        // Ghi lại kích thước ban đầu lúc Form vừa hiện (lúc này Windows đã chỉnh theo độ phân giải màn hình xong)
        private void Form_Shown(object? sender, EventArgs e)
        {
            originalClientSize = form.ClientSize;
            SaveOriginal(form.Controls);
        }

        private void SaveOriginal(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                originalBounds[control] = control.Bounds;
                originalFontSizes[control] = control.Font.Size;

                // Control nằm trong Panel cũng phải lưu
                SaveOriginal(control.Controls);
            }
        }

        private void Form_Resize(object? sender, EventArgs e)
        {
            // Form chưa hiện xong, hoặc đang thu nhỏ xuống thanh taskbar thì bỏ qua
            if (originalClientSize.Width == 0 || form.WindowState == FormWindowState.Minimized)
            {
                return;
            }

            float ratioX = (float)form.ClientSize.Width / originalClientSize.Width;
            float ratioY = (float)form.ClientSize.Height / originalClientSize.Height;
            ResizeControls(form.Controls, ratioX, ratioY);
        }

        private void ResizeControls(Control.ControlCollection controls, float ratioX, float ratioY)
        {
            foreach (Control control in controls)
            {
                if (!originalBounds.ContainsKey(control))
                {
                    continue;
                }

                // Control đã gắn Dock (ví dụ thanh công cụ) thì tự giãn theo cửa sổ, không cần đặt lại vị trí
                if (control.Dock == DockStyle.None)
                {
                    Rectangle old = originalBounds[control];
                    control.Bounds = new Rectangle(
                        (int)(old.X * ratioX),
                        (int)(old.Y * ratioY),
                        (int)(old.Width * ratioX),
                        (int)(old.Height * ratioY));
                }

                // Chữ to theo tỉ lệ nhỏ hơn trong 2 chiều để không bị tràn khung
                float fontSize = originalFontSizes[control] * Math.Min(ratioX, ratioY);
                control.Font = new Font(control.Font.FontFamily, fontSize, control.Font.Style);

                ResizeControls(control.Controls, ratioX, ratioY);
            }
        }
    }
}
