using System.Text.Json;

namespace CaroGame.BLL.Settings
{
    // Cấu hình riêng của từng máy, đọc từ file clientsettings.json nằm cạnh file .exe.
    // File này không commit lên git, mỗi người tự tạo từ clientsettings.example.json.
    public class ClientSettings
    {
        private const string FileName = "clientsettings.json";

        public string ServerUrl { get; set; } = "";
        public LlmSettings Llm { get; set; } = new LlmSettings();

        // Đọc file cấu hình. Thiếu file hoặc file sai định dạng thì dùng giá trị mặc định (rỗng),
        // khi đó chat với bot sẽ dùng câu soạn sẵn thay vì gọi LLM.
        public static ClientSettings Load()
        {
            string path = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(path))
            {
                return new ClientSettings();
            }

            try
            {
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<ClientSettings>(json) ?? new ClientSettings();
            }
            catch (JsonException)
            {
                return new ClientSettings();
            }
        }
    }

    public class LlmSettings
    {
        public string BaseUrl { get; set; } = "";
        public string Model { get; set; } = "";
        public string ApiKey { get; set; } = "";

        // Mức "suy nghĩ" trước khi trả lời. Gemini để "minimal" thì trả lời trong khoảng 1 giây.
        // Để trống thì không gửi (dùng khi đổi sang nhà cung cấp không hỗ trợ, ví dụ Groq).
        public string ReasoningEffort { get; set; } = "";

        // Đủ địa chỉ, tên model và API key thì mới gọi LLM được
        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(BaseUrl)
                    && !string.IsNullOrWhiteSpace(Model)
                    && !string.IsNullOrWhiteSpace(ApiKey);
            }
        }
    }
}
