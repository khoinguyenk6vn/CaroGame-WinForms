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

        public BotChatService(LlmSettings settings)
        {
            this.settings = settings;

            // Chờ LLM quá 10 giây thì bỏ, dùng câu soạn sẵn
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + settings.ApiKey);
        }

        // Chỉ gửi nội dung chat và tình hình trận, KHÔNG gửi thông tin cá nhân của người chơi
        public async Task<string> GetReplyAsync(string playerMessage, LocalMatch match)
        {
            string matchSituation = "Bàn cờ 20x20, đã đánh " + match.Board.MoveCount + " nước. Người chơi cầm X, bot cầm O.";

            if (!settings.IsConfigured)
            {
                return GetPresetReply();
            }

            try
            {
                return await AskLlmAsync(playerMessage, matchSituation);
            }
            catch (Exception)
            {
                // Mọi lỗi khi gọi LLM đều dùng câu soạn sẵn, game vẫn chơi bình thường
                return GetPresetReply();
            }
        }

        private string GetPresetReply()
        {
            return PresetReplies[random.Next(PresetReplies.Length)];
        }

        // Gọi API kiểu OpenAI (Gemini và Groq đều hỗ trợ): POST {BaseUrl}/chat/completions
        private async Task<string> AskLlmAsync(string playerMessage, string matchSituation)
        {
            string systemPrompt = "Bạn là bot trong game cờ caro, đang đấu với người chơi. "
                + "Trả lời bằng tiếng Việt, tối đa 1-2 câu ngắn. "
                + "Tình hình trận: " + matchSituation;

            var requestBody = new
            {
                model = settings.Model,
                max_tokens = 100,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = playerMessage }
                }
            };

            string url = settings.BaseUrl.TrimEnd('/') + "/chat/completions";
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            // Câu trả lời nằm ở choices[0].message.content
            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(json);
            return document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;
        }
    }
}
