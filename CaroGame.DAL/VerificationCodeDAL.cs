using CaroGame.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaroGame.DAL
{
    // Đọc/ghi bảng VerificationCodes bằng EF Core (LINQ to Entities).
    // Mỗi hàm tạo 1 AppDbContext mới từ factory, dùng xong thì hủy (using).
    public class VerificationCodeDAL
    {
        private readonly IDbContextFactory<AppDbContext> dbFactory;

        public VerificationCodeDAL(IDbContextFactory<AppDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        // Lấy mã của 1 user cho 1 mục đích, null nếu không có
        public async Task<VerificationCode?> GetAsync(int userId, string purpose)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            return await db.VerificationCodes.FirstOrDefaultAsync(c => c.UserId == userId && c.Purpose == purpose);
        }

        public async Task AddAsync(VerificationCode code)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            db.VerificationCodes.Add(code);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(VerificationCode code)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            db.VerificationCodes.Update(code);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(VerificationCode code)
        {
            using AppDbContext db = dbFactory.CreateDbContext();
            db.VerificationCodes.Remove(code);
            await db.SaveChangesAsync();
        }
    }
}
