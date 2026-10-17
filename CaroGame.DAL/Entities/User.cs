using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.DAL.Entities
{
    // Bảng Users. Username và Email không được trùng.
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Username { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";

        // Mật khẩu đã hash bằng BCrypt, không lưu mật khẩu gốc
        public string PasswordHash { get; set; } = "";

        public bool IsEmailVerified { get; set; }

        public int Score { get; set; }

        // Số thứ tự ảnh avatar mặc định (0-19) đóng gói sẵn trong client
        public int DefaultAvatarIndex { get; set; }

        // Đường dẫn ảnh avatar user tự upload, null nếu chưa upload
        [MaxLength(260)]
        public string? AvatarPath { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
