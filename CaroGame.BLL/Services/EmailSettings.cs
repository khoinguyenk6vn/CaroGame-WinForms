namespace CaroGame.BLL.Services
{
    // Cấu hình gửi mail, Server đọc từ appsettings.Local.json rồi truyền vào
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string Address { get; set; } = "";
        public string AppPassword { get; set; } = "";
    }
}
