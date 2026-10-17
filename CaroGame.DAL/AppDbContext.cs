using CaroGame.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.DAL
{
    // Code First: EF Core tạo bảng trong SQL Server từ các class entity khai báo ở đây
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // EF Core tự gán giá trị cho các DbSet nên dùng null! để tắt cảnh báo null
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;
        public DbSet<MatchHistory> MatchHistories { get; set; } = null!;
    }
}
