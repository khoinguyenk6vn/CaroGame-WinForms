using System.ComponentModel.DataAnnotations;

namespace CaroGame.DAL.Entities
{
    // Bảng VerificationCodes: mã 6 số gửi qua email.
    // Mỗi user chỉ có tối đa 1 mã cho mỗi mục đích, gửi mã mới thì mã cũ bị xóa.
    public class VerificationCode
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [Required(ErrorMessage = "Thiếu mã xác minh.")]
        [MaxLength(6)]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Mã xác minh phải gồm đúng 6 chữ số.")]
        public string Code { get; set; } = "";

        // Mục đích của mã: "Register" (đăng ký) hoặc "ResetPassword" (quên mật khẩu)
        [Required(ErrorMessage = "Thiếu mục đích của mã.")]
        [MaxLength(20)]
        public string Purpose { get; set; } = "";

        // Thời điểm gửi mã, dùng để bắt chờ 60 giây giữa 2 lần gửi
        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        // Số lần nhập sai, sai 5 lần thì mã bị hủy
        public int FailedAttempts { get; set; }
    }
}
