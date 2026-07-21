using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IEmailService _emailService;

        public ContactController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                try
                {
                    await _emailService.SendEmailAsync(subject, body);
                }
                catch (Exception ex)
                {
                    // Email logging is handled inside EmailService
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
