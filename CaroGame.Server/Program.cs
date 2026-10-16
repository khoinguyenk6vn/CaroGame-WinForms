using CaroGame.Server.Data;
using CaroGame.Server.Hubs;
using CaroGame.Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// File cấu hình riêng của từng máy (connection string, Gmail App Password).
// File này nằm trong .gitignore, mỗi thành viên tự tạo từ appsettings.Local.example.json.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

// Đăng ký các class để ASP.NET Core tự tạo và truyền vào constructor khi cần
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Mở địa chỉ server trên trình duyệt để kiểm tra server có đang chạy không
app.MapGet("/", () => "Caro server dang chay");

// Client kết nối SignalR tới địa chỉ <server>/gamehub
app.MapHub<GameHub>("/gamehub");

app.Run();
