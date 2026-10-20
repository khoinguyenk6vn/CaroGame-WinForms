using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.DAL.Entities
{
    // Bảng Users. Username và Email không được trùng.
    // Các thuộc tính [Required], [MaxLength], [Range]... vừa để EF Core tạo cột đúng kiểu,
    // vừa để EntityValidator kiểm tra dữ liệu trước khi gọi SaveChangesAsync (ErrorMessage là câu báo lỗi).
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username không được để trống.")]
        [MaxLength(20, ErrorMessage = "Username tối đa 20 ký tự.")]
        [RegularExpression("^[A-Za-z0-9_]{4,20}$", ErrorMessage = "Username phải dài 4-20 ký tự, chỉ gồm chữ không dấu, số và dấu _.")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Email không được để trống.")]
        [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = "";

        // Mật khẩu đã hash bằng BCrypt, không lưu mật khẩu gốc
        [Required(ErrorMessage = "Thiếu mật khẩu.")]
        public string PasswordHash { get; set; } = "";

        public bool IsEmailVerified { get; set; }

        // Điểm khởi đầu 1000 và không bao giờ dưới 1000
        [Range(1000, int.MaxValue, ErrorMessage = "Điểm không được dưới 1000.")]
        public int Score { get; set; }

        // Số thứ tự ảnh avatar mặc định (0-19) đóng gói sẵn trong client
        [Range(0, 19, ErrorMessage = "Số thứ tự avatar mặc định phải từ 0 đến 19.")]
        public int DefaultAvatarIndex { get; set; }

        // Đường dẫn ảnh avatar user tự upload, null nếu chưa upload
        [MaxLength(260, ErrorMessage = "Đường dẫn avatar quá dài.")]
        public string? AvatarPath { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
