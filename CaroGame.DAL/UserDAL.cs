using CaroGame.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.DAL
{
    // Đọc/ghi bảng Users bằng EF Core (LINQ to Entities).
    // Mỗi hàm tạo 1 AppDbContext mới từ factory, dùng xong thì hủy (using),
    // nên class này dùng được cùng lúc ở nhiều luồng.
    public class UserDAL
    {
        private readonly IDbContextFactory<AppDbContext> dbFactory;

        public UserDAL(IDbContextFactory<AppDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        // SQL Server mặc định so sánh chuỗi không phân biệt hoa thường,
        // nên tìm "abc" cũng ra tài khoản "Abc"
        public async Task<User?> GetByUsernameAsync(string username)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }

        // Lưu các thay đổi trên một user (đổi mật khẩu, xác minh email, đổi điểm, đổi avatar...)
        public async Task UpdateAsync(User user)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }

        // Xếp hạng mọi tài khoản đã xác minh: điểm cao trước,
        // bằng điểm thì nhiều trận thắng Online trước, rồi tới username theo chữ cái.
        // Số trận thắng không lưu cột riêng mà đếm từ bảng lịch sử.
        public async Task<List<UserRanking>> GetRankingAsync()
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.Users
                .Where(u => u.IsEmailVerified)
                .Select(u => new UserRanking
                {
                    UserId = u.Id,
                    Username = u.Username,
                    Score = u.Score,
                    OnlineWins = db.MatchHistories.Count(h => h.UserId == u.Id && h.Mode == "PvPOnline" && h.Result == "Win")
                })
                .OrderByDescending(r => r.Score)
                .ThenByDescending(r => r.OnlineWins)
                .ThenBy(r => r.Username)
                .ToListAsync();
        }
    }
}
