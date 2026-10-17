using CaroGame.BLL.Dtos;
using CaroGame.DAL;
using CaroGame.DAL.Entities;

namespace CaroGame.BLL.Services
{
    // Lưu ảnh avatar user upload. File ảnh nằm trong thư mục của server, CSDL chỉ lưu đường dẫn.
    public class AvatarService
    {
        private const int MaxFileBytes = 1024 * 1024;

        private readonly UserDAL userDal;
        private readonly AvatarSettings settings;

        public AvatarService(UserDAL userDal, AvatarSettings settings)
        {
            this.userDal = userDal;
            this.settings = settings;
        }

        // token: session token của người upload (endpoint HTTP không có mã kết nối SignalR)
        public async Task<AccountResult> SaveAvatarAsync(string token, string fileName, byte[] content)
        {
            OnlineUser? onlineUser = SessionStore.FindByToken(token);
            if (onlineUser == null)
            {
                return Fail("Phiên đăng nhập không hợp lệ, hãy đăng nhập lại.");
            }

            string extension = Path.GetExtension(fileName).ToLower();
            if (extension != ".png" && extension != ".jpg")
            {
                return Fail("Chỉ nhận ảnh .png hoặc .jpg.");
            }

            if (content.Length > MaxFileBytes)
            {
                return Fail("Ảnh tối đa 1MB.");
            }

            User? user = await userDal.GetByIdAsync(onlineUser.UserId);
            if (user == null)
            {
                return Fail("Tài khoản không tồn tại.");
            }

            // Upload ảnh mới thì xóa file ảnh cũ
            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                string oldFile = Path.Combine(settings.Folder, Path.GetFileName(user.AvatarPath));
                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                }
            }

            // Mỗi lần upload đặt tên file mới (kèm thời điểm) để client không dùng nhầm ảnh cũ
            string newFileName = user.Id + "_" + DateTime.Now.Ticks + extension;
            await File.WriteAllBytesAsync(Path.Combine(settings.Folder, newFileName), content);

            user.AvatarPath = "avatars/" + newFileName;
            await userDal.UpdateAsync(user);

            return new AccountResult { Success = true, Message = "Đổi avatar thành công." };
        }

        private static AccountResult Fail(string message)
        {
            return new AccountResult { Success = false, Message = message };
        }
    }
}
