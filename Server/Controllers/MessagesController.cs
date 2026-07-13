using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/messages
        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var list = await _context.Messages
                .OrderByDescending(m => m.Date)
                .ToListAsync();
            return Ok(list);
        }

        // PUT: api/messages/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> ToggleReadStatus(int id, [FromQuery] bool isRead)
        {
            var msg = await _context.Messages.FindAsync(id);
            if (msg == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            msg.IsRead = isRead;
            _context.Entry(msg).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(msg);
        }

        // DELETE: api/messages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var msg = await _context.Messages.FindAsync(id);
            if (msg == null)
            {
                return NotFound("Mesaj bulunamadı.");
            }

            _context.Messages.Remove(msg);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
