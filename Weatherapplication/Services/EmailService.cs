using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Weatherapplication.Models;

namespace Weatherapplication.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
            if (string.IsNullOrEmpty(_settings.SenderEmail))
            {
                throw new Exception("SenderEmail is NULL");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MailMessage();

            message.From = new MailAddress(_settings.SenderEmail, _settings.SenderName);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient(_settings.SmtpServer, _settings.Port))
            {
                smtp.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                smtp.EnableSsl = _settings.EnableSsl;
                await smtp.SendMailAsync(message);
            }
        }
    }
}
