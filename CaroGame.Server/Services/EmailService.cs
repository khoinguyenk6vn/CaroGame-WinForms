using System.Net;
using System.Net.Mail;

namespace CaroGame.Server.Services
{
    // Gửi mã xác minh qua Gmail SMTP.
    // Địa chỉ Gmail và App Password nằm trong appsettings.Local.json (không commit lên git).
    public class EmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendCodeAsync(string toEmail, string code)
        {
            string host = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
            int port = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            string fromAddress = configuration["Email:Address"] ?? "";
            string appPassword = configuration["Email:AppPassword"] ?? "";

            // Chưa cấu hình Gmail (khi đang code thử trên máy mình): in mã ra cửa sổ server thay vì gửi mail
            if (fromAddress == "" || appPassword == "")
            {
                Console.WriteLine("[Ma xac minh] " + toEmail + ": " + code);
                return;
            }

            using var smtp = new SmtpClient(host, port);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(fromAddress, appPassword);

            using var mail = new MailMessage(fromAddress, toEmail);
            mail.Subject = "Mã xác minh game Caro";
            mail.Body = "Mã xác minh của bạn là: " + code + "\nMã hết hạn sau 5 phút.";

            await smtp.SendMailAsync(mail);
        }
    }
}
