using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Server.Data;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public ContactController(AppDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        // POST: api/contact
        [HttpPost]
        public async Task<IActionResult> SubmitContactMessage([FromBody] MessageDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest("Lütfen tüm alanları doldurun.");
            }

            var messageEntity = new MessageEntity
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Message = dto.Message.Trim(),
                Date = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(messageEntity);
            await _context.SaveChangesAsync();

            // Send email notification in the background without blocking the HTTP response
            var subject = "[Portfolio] Yeni Bir Mesajınız Var!";
            var body = "Yeni bir mesajınız var!\n\n" +
                       $"Gönderen: {messageEntity.Name} ({messageEntity.Email})\n" +
                       $"Tarih: {messageEntity.Date:g} UTC\n\n" +
                       $"Mesaj:\n{messageEntity.Message}";

            _ = Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    try
                    {
                        await emailService.SendEmailAsync(subject, body);
                    }
                    catch (Exception)
                    {
                        // Email logging is handled inside EmailService
                    }
                }
            });

            return Ok(new { success = true, id = messageEntity.Id });
        }
    }

    public class MessageDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
