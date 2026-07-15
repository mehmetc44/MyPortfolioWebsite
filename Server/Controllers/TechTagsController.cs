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
    public class TechTagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TechTagsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/techtags
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetTechTags()
        {
            var tags = await _context.TechTags.OrderBy(t => t.Id).ToListAsync();
            return Ok(tags);
        }

        // POST: api/techtags
        [HttpPost]
        public async Task<IActionResult> CreateTechTag([FromBody] TechTagEntity tag)
        {
            if (tag == null || string.IsNullOrWhiteSpace(tag.Name))
            {
                return BadRequest("Geçersiz teknoloji etiketi verisi.");
            }

            _context.TechTags.Add(tag);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTechTags), new { id = tag.Id }, tag);
        }

        // PUT: api/techtags/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTechTag(int id, [FromBody] TechTagEntity updatedTag)
        {
            if (updatedTag == null || id != updatedTag.Id)
            {
                return BadRequest("ID uyuşmazlığı.");
            }

            var existing = await _context.TechTags.FindAsync(id);
            if (existing == null)
            {
                return NotFound("Teknoloji etiketi bulunamadı.");
            }

            existing.Name = updatedTag.Name;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // DELETE: api/techtags/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTechTag(int id)
        {
            var tag = await _context.TechTags.FindAsync(id);
            if (tag == null)
            {
                return NotFound("Teknoloji etiketi bulunamadı.");
            }

            _context.TechTags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
