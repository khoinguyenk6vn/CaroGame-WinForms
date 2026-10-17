using System.Net;
using System.Text;
using System.Text.Json;
using CaroGame.BLL.Game;

namespace CaroGame.BLL.Services
{
    // Chat với bot ở chế độ PvE.
    // Có cấu hình LLM và gọi được thì dùng câu trả lời của LLM,
    // còn lại (chưa cấu hình, mất mạng, hết lượt 429, timeout...) thì dùng câu soạn sẵn.
    public class BotChatService
    {
        private const int MaxAttempts = 2;

        private static readonly string[] PresetReplies =
        {
            "Nước đi hay đấy, nhưng tôi vẫn đang để ý bạn!",
            "Hừm, để tôi suy nghĩ một chút...",
            "Bạn chơi khá đấy, cẩn thận đường chéo nhé!",
            "Ván này còn dài, đừng vội mừng!",
            "Chúc bạn may mắn!"
        };

        private readonly LlmSettings settings;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly Random random = new Random();

        // Lý do lần gọi gần nhất không dùng được LLM (rỗng nếu gọi thành công), để Form báo cho người chơi
        public string LastError { get; private set; } = "";

        public BotChatService(LlmSettings settings)
        {
            this.settings = settings;

            // Chờ LLM quá 12 giây thì bỏ, dùng câu soạn sẵn
            httpClient.Timeout = TimeSpan.FromSeconds(12);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + settings.ApiKey);
        }

        // Chỉ gửi nội dung chat và tình hình trận, KHÔNG gửi thông tin cá nhân của người chơi
        public async Task<string> GetReplyAsync(string playerMessage, LocalMatch match)
        {
            string matchSituation = "Bàn cờ 20x20, đã đánh " + match.Board.MoveCount + " nước. Người chơi cầm X, bot cầm O.";

            if (!settings.IsConfigured)
            {
                LastError = "chưa có API key trong file clientsettings.json";
                return GetPresetReply();
            }

            // Máy chủ AI miễn phí đôi khi quá tải (lỗi 503), thử lại thêm 1 lần thường là được
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    string reply = await AskLlmAsync(playerMessage, matchSituation);
                    if (reply != "")
                    {
                        LastError = "";
                        return reply;
                    }
                    LastError = "AI không trả lời nội dung nào";
                }
                catch (HttpRequestException ex)
                {
                    LastError = DescribeHttpError(ex.StatusCode);
                }
                catch (TaskCanceledException)
                {
                    LastError = "AI trả lời quá lâu (quá 12 giây)";
                }
                catch (JsonException)
                {
                    LastError = "câu trả lời của AI không đúng định dạng";
                }
            }

            // Mọi lỗi khi gọi LLM đều dùng câu soạn sẵn, game vẫn chơi bình thường
            return GetPresetReply();
        }

        private string GetPresetReply()
        {
            return PresetReplies[random.Next(PresetReplies.Length)];
        }

        // Đổi mã lỗi HTTP ra lý do dễ hiểu
        private string DescribeHttpError(HttpStatusCode? statusCode)
        {
            if (statusCode == null)
            {
                return "không kết nối được máy chủ AI (kiểm tra mạng)";
            }

            int code = (int)statusCode.Value;
            if (code == 429)
            {
                return "API key đã hết lượt gọi miễn phí (lỗi 429), thử lại sau ít phút";
            }
            if (code == 503)
            {
                return "máy chủ AI đang quá tải (lỗi 503)";
            }
            if (code == 400 || code == 401 || code == 403 || code == 404)
            {
                return "API key hoặc tên model trong clientsettings.json không đúng (lỗi " + code + ")";
            }
            return "máy chủ AI báo lỗi " + code;
        }

        // Gọi API kiểu OpenAI (Gemini và Groq đều hỗ trợ): POST {BaseUrl}/chat/completions.
        // Trả về "" nếu AI không có nội dung trả lời.
        private async Task<string> AskLlmAsync(string playerMessage, string matchSituation)
        {
            string systemPrompt = "Bạn là bot trong game cờ caro, đang đấu với người chơi. "
                + "Trả lời bằng tiếng Việt, tối đa 1-2 câu ngắn. "
                + "Tình hình trận: " + matchSituation;

            // max_tokens đặt rộng vì model có "suy nghĩ" cũng tốn token trước khi trả lời;
            // câu trả lời vẫn ngắn nhờ yêu cầu "tối đa 1-2 câu" ở trên
            var requestBody = new Dictionary<string, object>
            {
                ["model"] = settings.Model,
                ["max_tokens"] = 1024,
                ["messages"] = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = playerMessage }
                }
            };

            if (settings.ReasoningEffort != "")
            {
                requestBody["reasoning_effort"] = settings.ReasoningEffort;
            }

            string url = settings.BaseUrl.TrimEnd('/') + "/chat/completions";
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            // Câu trả lời nằm ở choices[0].message.content
            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement message = document.RootElement.GetProperty("choices")[0].GetProperty("message");

            if (!message.TryGetProperty("content", out JsonElement reply) || reply.ValueKind != JsonValueKind.String)
            {
                return "";
            }

            return reply.GetString()!.Trim();
        }
    }
}
