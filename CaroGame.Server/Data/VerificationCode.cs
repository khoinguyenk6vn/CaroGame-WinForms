using System.ComponentModel.DataAnnotations;
using CaroGame.Shared.Dtos;

namespace CaroGame.Server.Data
{
    // Bảng VerificationCodes: mã 6 số gửi qua email.
    // Mỗi user chỉ có tối đa 1 mã cho mỗi mục đích, gửi mã mới thì mã cũ bị xóa.
    public class VerificationCode
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [MaxLength(6)]
        public string Code { get; set; } = "";

        public CodePurpose Purpose { get; set; }

        // Thời điểm gửi mã, dùng để bắt chờ 60 giây giữa 2 lần gửi
        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        // Số lần nhập sai, sai 5 lần thì mã bị hủy
        public int FailedAttempts { get; set; }
    }
}
