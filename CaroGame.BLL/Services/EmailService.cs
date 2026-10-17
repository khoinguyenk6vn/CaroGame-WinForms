using System.Net;
using System.Net.Mail;
using CaroGame.BLL.Settings;

namespace CaroGame.BLL.Services
{
    // Gửi mã xác minh qua Gmail SMTP
    public class EmailService
    {
        private readonly EmailSettings settings;

        public EmailService(EmailSettings settings)
        {
            this.settings = settings;
        }

        public async Task SendCodeAsync(string toEmail, string code)
        {
            // Chưa cấu hình Gmail (khi đang code thử trên máy mình): in mã ra cửa sổ server thay vì gửi mail
            if (settings.Address == "" || settings.AppPassword == "")
            {
                Console.WriteLine("[Ma xac minh] " + toEmail + ": " + code);
                return;
            }

            using var smtp = new SmtpClient(settings.SmtpHost, settings.SmtpPort);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(settings.Address, settings.AppPassword);

            using var mail = new MailMessage(settings.Address, toEmail);
            mail.Subject = "Mã xác minh game Caro";
            mail.Body = "Mã xác minh của bạn là: " + code + "\nMã hết hạn sau 5 phút.";

            await smtp.SendMailAsync(mail);
        }
    }
}
