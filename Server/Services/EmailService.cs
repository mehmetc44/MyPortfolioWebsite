using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string subject, string body)
        {
            var host = _config["SmtpSettings:Host"];
            var portStr = _config["SmtpSettings:Port"];
            var enableSslStr = _config["SmtpSettings:EnableSsl"];
            var senderName = _config["SmtpSettings:SenderName"];
            var senderEmail = _config["SmtpSettings:SenderEmail"];
            var senderPassword = _config["SmtpSettings:SenderPassword"];
            var receiverEmail = _config["SmtpSettings:ReceiverEmail"] ?? "cakmakm541@gmail.com";

            int port = 587;
            int.TryParse(portStr, out port);

            bool enableSsl = true;
            bool.TryParse(enableSslStr, out enableSsl);

            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
            {
                _logger.LogWarning("SMTP credentials are not fully configured (SenderEmail or SenderPassword is empty). Fallback email details:\nReceiver: {Receiver}\nSubject: {Subject}\nBody:\n{Body}", receiverEmail, subject, body);
                return;
            }

            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail, senderName);
                    mail.To.Add(new MailAddress(receiverEmail));
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    using (var smtp = new SmtpClient(host, port))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        smtp.EnableSsl = enableSsl;
                        await smtp.SendMailAsync(mail);
                        _logger.LogInformation("Email successfully sent to {Receiver} via SMTP.", receiverEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP Email delivery failed. Fallback email details:\nReceiver: {Receiver}\nSubject: {Subject}\nBody:\n{Body}", receiverEmail, subject, body);
            }
        }
    }
}
