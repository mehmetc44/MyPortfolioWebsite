using System;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendEmailAsync(string subject, string body)
        {
            var receiverEmail = _config["SmtpSettings:ReceiverEmail"] ?? "cakmakm541@gmail.com";
            var resendApiKey = _config["ResendApiKey"] ?? _config["SmtpSettings:ResendApiKey"];

            // Try Resend API first (works on Render free tier since it uses port 443/HTTPS)
            if (!string.IsNullOrWhiteSpace(resendApiKey))
            {
                try
                {
                    using (var client = _httpClientFactory.CreateClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", resendApiKey.Trim());
                        
                        var payload = new
                        {
                            from = "Portfolio Contact <onboarding@resend.dev>",
                            to = new[] { receiverEmail },
                            subject = subject,
                            text = body
                        };

                        var json = JsonSerializer.Serialize(payload);
                        using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
                        {
                            var response = await client.PostAsync("https://api.resend.com/emails", content);
                            if (response.IsSuccessStatusCode)
                            {
                                _logger.LogInformation("Email successfully sent to {Receiver} via Resend API.", receiverEmail);
                                return;
                            }
                            else
                            {
                                var errText = await response.Content.ReadAsStringAsync();
                                _logger.LogError("Resend API failed with status {StatusCode}: {Error}. Falling back to SMTP.", response.StatusCode, errText);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email via Resend API. Falling back to SMTP.");
                }
            }

            // Fallback to traditional SMTP (blocked on Render free tier, but works locally)
            var host = _config["SmtpSettings:Host"];
            var portStr = _config["SmtpSettings:Port"];
            var enableSslStr = _config["SmtpSettings:EnableSsl"];
            var senderName = _config["SmtpSettings:SenderName"];
            var senderEmail = _config["SmtpSettings:SenderEmail"];
            var senderPassword = _config["SmtpSettings:SenderPassword"];

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
