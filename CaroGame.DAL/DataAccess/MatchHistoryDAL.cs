using CaroGame.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.DAL.DataAccess
{
    // Đọc/ghi bảng MatchHistories bằng EF Core (LINQ to Entities).
    // Mỗi hàm tạo 1 AppDbContext mới từ factory, dùng xong thì hủy (using).
    public class MatchHistoryDAL
    {
        private readonly IDbContextFactory<AppDbContext> dbFactory;

        public MatchHistoryDAL(IDbContextFactory<AppDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task AddAsync(MatchHistory history)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            db.MatchHistories.Add(history);
            await db.SaveChangesAsync();
        }

        // Lịch sử của 1 người, trận mới nhất lên đầu. Include để lấy luôn tên đối thủ.
        public async Task<List<MatchHistory>> GetByUserAsync(int userId)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.MatchHistories
                .Include(h => h.Opponent)
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.PlayedAt)
                .ToListAsync();
        }
    }
}
