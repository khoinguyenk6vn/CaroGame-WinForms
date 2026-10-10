var builder = WebApplication.CreateBuilder(args);

// File cấu hình riêng của từng máy (connection string, Gmail App Password).
// File này nằm trong .gitignore, mỗi thành viên tự tạo từ appsettings.Local.example.json.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
