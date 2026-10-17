using CaroGame.BLL.Dtos;
using CaroGame.BLL.Online;
using CaroGame.BLL.Services;
using CaroGame.DAL;
using CaroGame.Server;
using CaroGame.Server.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// File cấu hình riêng của từng máy (connection string, Gmail App Password).
// File này nằm trong .gitignore, mỗi thành viên tự tạo từ appsettings.Local.example.json.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

// Đăng ký các class để ASP.NET Core tự tạo và truyền vào constructor khi cần.
// Server chỉ khai báo kết nối CSDL ở đây, mọi truy vấn nằm ở tầng DAL.
// Dùng DbContextFactory: mỗi hàm DAL tự tạo 1 AppDbContext mới rồi hủy.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Tầng DAL
builder.Services.AddSingleton<UserDAL>();
builder.Services.AddSingleton<VerificationCodeDAL>();
builder.Services.AddSingleton<MatchHistoryDAL>();

// Tầng BLL
var emailSettings = new EmailSettings
{
    SmtpHost = builder.Configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
    SmtpPort = int.Parse(builder.Configuration["Email:SmtpPort"] ?? "587"),
    Address = builder.Configuration["Email:Address"] ?? "",
    AppPassword = builder.Configuration["Email:AppPassword"] ?? ""
};
builder.Services.AddSingleton(emailSettings);

// Ảnh avatar upload lưu trong thư mục wwwroot/avatars của server
string avatarFolder = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "avatars");
Directory.CreateDirectory(avatarFolder);
builder.Services.AddSingleton(new AvatarSettings { Folder = avatarFolder });

builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<PlayerService>();
builder.Services.AddSingleton<AvatarService>();
builder.Services.AddSingleton<OnlineGameManager>();

// BLL gửi tin cho client qua SignalRNotifier; GameTickService gọi OnlineGameManager mỗi giây
builder.Services.AddSingleton<IClientNotifier, SignalRNotifier>();
builder.Services.AddHostedService<GameTickService>();

builder.Services.AddSignalR();

var app = builder.Build();

// Cho phép client tải ảnh avatar trong wwwroot, ví dụ <server>/avatars/12_638...png
app.UseStaticFiles();

// Mở địa chỉ server trên trình duyệt để kiểm tra server có đang chạy không
app.MapGet("/", () => "Caro server dang chay");

// Upload avatar: client gửi file ảnh kèm session token trong header X-Session-Token
app.MapPost("/avatar", async (HttpRequest request, AvatarService avatarService) =>
{
    IFormCollection form = await request.ReadFormAsync();
    IFormFile? file = form.Files.FirstOrDefault();
    if (file == null)
    {
        return Results.Text("Chưa chọn ảnh.", statusCode: 400);
    }

    using var memory = new MemoryStream();
    await file.CopyToAsync(memory);

    // Trả về câu thông báo dạng chữ, mã 200 nếu thành công, 400 nếu bị từ chối
    string token = request.Headers["X-Session-Token"].ToString();
    AccountResult result = await avatarService.SaveAvatarAsync(token, file.FileName, memory.ToArray());
    return Results.Text(result.Message, statusCode: result.Success ? 200 : 400);
});

// Client kết nối SignalR tới địa chỉ <server>/gamehub
app.MapHub<GameHub>("/gamehub");

app.Run();
