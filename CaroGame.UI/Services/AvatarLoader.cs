using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using CaroGame.BLL.Dtos;
using CaroGame.BLL.Services;

namespace CaroGame.UI.Services
{
    // Lấy ảnh avatar để hiện lên PictureBox, và upload ảnh avatar mới lên server
    public static class AvatarLoader
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Ảnh của người đang chơi: đã upload thì tải từ server, chưa thì dùng ảnh mặc định.
        // Trả về null nếu không có ảnh nào (khi đó giữ nguyên ảnh đang hiện).
        public static async Task<Image?> LoadForCurrentPlayerAsync()
        {
            if (Session.AvatarPath != "")
            {
                try
                {
                    byte[] bytes = await httpClient.GetByteArrayAsync(ServerConnection.ServerUrl + "/" + Session.AvatarPath);
                    return Image.FromStream(new MemoryStream(bytes));
                }
                catch (Exception)
                {
                    // Không tải được thì dùng ảnh mặc định bên dưới
                }
            }

            return LoadDefault(Session.DefaultAvatarIndex);
        }

        // 20 ảnh mặc định nằm trong thư mục Avatars cạnh file .exe (avatar_0.png ... avatar_19.png)
        public static Image? LoadDefault(int index)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Avatars", "avatar_" + index + ".png");
            if (!File.Exists(path))
            {
                return null;
            }
            return Image.FromFile(path);
        }

        // Gửi file ảnh lên server kèm session token.
        // Trả về kết quả: Success = true nếu server nhận ảnh (mã 200), Message là câu thông báo của server.
        public static async Task<AccountResult> UploadAsync(string filePath)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                using var request = new HttpRequestMessage(HttpMethod.Post, ServerConnection.ServerUrl + "/avatar");
                request.Headers.Add("X-Session-Token", Session.Token);
                request.Content = form;

                HttpResponseMessage response = await httpClient.SendAsync(request);
                string message = await response.Content.ReadAsStringAsync();
                return new AccountResult { Success = response.IsSuccessStatusCode, Message = message };
            }
            catch (Exception)
            {
                return new AccountResult { Success = false, Message = "Không kết nối được server." };
            }
        }
    }
}
