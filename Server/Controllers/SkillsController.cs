using System.Collections.Generic;
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
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/skills
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSkills()
        {
            var skills = await _context.Skills.OrderBy(s => s.OrderIndex).ThenBy(s => s.Id).ToListAsync();
            return Ok(skills);
        }

        // POST: api/skills
        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] SkillEntity skill)
        {
            if (skill == null || string.IsNullOrWhiteSpace(skill.Name))
            {
                return BadRequest("Geçersiz yetenek verisi.");
            }

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSkills), new { id = skill.Id }, skill);
        }

        // PUT: api/skills/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] SkillEntity updatedSkill)
        {
            if (updatedSkill == null || id != updatedSkill.Id)
            {
                return BadRequest("ID uyuşmazlığı.");
            }

            var existing = await _context.Skills.FindAsync(id);
            if (existing == null)
            {
                return NotFound("Yetenek bulunamadı.");
            }

            existing.Name = updatedSkill.Name;
            existing.Percentage = updatedSkill.Percentage;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // PUT: api/skills/reorder
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderSkills([FromBody] List<SkillReorderItem> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest("Geçersiz sıralama verisi.");

            foreach (var item in items)
            {
                var skill = await _context.Skills.FindAsync(item.Id);
                if (skill != null)
                {
                    skill.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/skills/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound("Yetenek bulunamadı.");
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class SkillReorderItem
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
    }
}
