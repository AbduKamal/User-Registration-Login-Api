using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace UserApi.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _emailFrom = "abdulrmankamal@gmail.com";
        private readonly string _emailPassword = "couvnzpatmlruotm";

        public async Task SendVerificationEmail(string email, int code)
        {
            using var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_emailFrom, _emailPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailFrom),
                Subject = "Email Verification Code",
                Body = $"Your verification code is: <b>{code}</b>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
