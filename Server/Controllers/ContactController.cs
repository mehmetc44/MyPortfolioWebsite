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

            // Send email notification in the background
            var subject = "[Portfolio] Yeni Bir Mesajınız Var!";
            var body = "Yeni bir mesajınız var!\n\n" +
                       $"Gönderen: {messageEntity.Name} ({messageEntity.Email})\n" +
                       $"Tarih: {messageEntity.Date:g} UTC\n\n" +
                       $"Mesaj:\n{messageEntity.Message}";

            // We await email delivery. It will log internally if it fails, so it won't crash this request.
            await _emailService.SendEmailAsync(subject, body);

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
