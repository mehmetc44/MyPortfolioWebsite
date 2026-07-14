using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/profile/raw
        [HttpGet("raw")]
        public async Task<IActionResult> GetRawProfile()
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null)
            {
                return NotFound("Profil bulunamadı.");
            }
            return Ok(profile);
        }

        // GET: api/profile?lang=tr
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProfile([FromQuery] string lang = "tr")
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null)
            {
                return NotFound("Profil bulunamadı.");
            }

            lang = lang.ToLower();
            var mapped = new
            {
                Id = profile.Id,
                Name = profile.Name,
                AvatarUrl = profile.AvatarUrl,
                Repos = profile.Repos,
                Pubs = profile.Pubs,
                Github = profile.Github,
                Linkedin = profile.Linkedin,
                Instagram = profile.Instagram,
                Medium = profile.Medium,
                Tag = lang == "en" ? profile.Tag_EN : (lang == "de" ? profile.Tag_DE : profile.Tag_TR),
                Title = lang == "en" ? profile.Title_EN : (lang == "de" ? profile.Title_DE : profile.Title_TR),
                Bio = lang == "en" ? profile.Bio_EN : (lang == "de" ? profile.Bio_DE : profile.Bio_TR),
                CvText = lang == "en" ? profile.CvText_EN : (lang == "de" ? profile.CvText_DE : profile.CvText_TR),
                CvPdfUrl = lang == "en" ? profile.CvPdfUrl_EN : (lang == "de" ? profile.CvPdfUrl_DE : profile.CvPdfUrl_TR),
                CvPdfUrl_TR = profile.CvPdfUrl_TR,
                CvPdfUrl_EN = profile.CvPdfUrl_EN,
                CvPdfUrl_DE = profile.CvPdfUrl_DE
            };

            return Ok(mapped);
        }

        // PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileEntity updatedProfile)
        {
            var existingProfile = await _context.Profiles.FirstOrDefaultAsync();
            if (existingProfile == null)
            {
                return NotFound("Güncellenecek profil bulunamadı.");
            }

            // Map values with fallback to existing values to avoid null insertions
            existingProfile.Name = updatedProfile.Name ?? existingProfile.Name;
            existingProfile.Tag_TR = updatedProfile.Tag_TR ?? existingProfile.Tag_TR;
            existingProfile.Tag_EN = updatedProfile.Tag_EN ?? existingProfile.Tag_EN;
            existingProfile.Tag_DE = updatedProfile.Tag_DE ?? existingProfile.Tag_DE;
            existingProfile.Title_TR = updatedProfile.Title_TR ?? existingProfile.Title_TR;
            existingProfile.Title_EN = updatedProfile.Title_EN ?? existingProfile.Title_EN;
            existingProfile.Title_DE = updatedProfile.Title_DE ?? existingProfile.Title_DE;
            existingProfile.Bio_TR = updatedProfile.Bio_TR ?? existingProfile.Bio_TR;
            existingProfile.Bio_EN = updatedProfile.Bio_EN ?? existingProfile.Bio_EN;
            existingProfile.Bio_DE = updatedProfile.Bio_DE ?? existingProfile.Bio_DE;
            existingProfile.AvatarUrl = updatedProfile.AvatarUrl ?? existingProfile.AvatarUrl;
            existingProfile.Repos = updatedProfile.Repos;
            existingProfile.Pubs = updatedProfile.Pubs;
            existingProfile.Github = updatedProfile.Github ?? existingProfile.Github;
            existingProfile.Linkedin = updatedProfile.Linkedin ?? existingProfile.Linkedin;
            existingProfile.Instagram = updatedProfile.Instagram ?? existingProfile.Instagram;
            existingProfile.Medium = updatedProfile.Medium ?? existingProfile.Medium;
            existingProfile.CvText_TR = updatedProfile.CvText_TR ?? existingProfile.CvText_TR;
            existingProfile.CvText_EN = updatedProfile.CvText_EN ?? existingProfile.CvText_EN;
            existingProfile.CvText_DE = updatedProfile.CvText_DE ?? existingProfile.CvText_DE;
            existingProfile.CvPdfUrl_TR = updatedProfile.CvPdfUrl_TR ?? existingProfile.CvPdfUrl_TR;
            existingProfile.CvPdfUrl_EN = updatedProfile.CvPdfUrl_EN ?? existingProfile.CvPdfUrl_EN;
            existingProfile.CvPdfUrl_DE = updatedProfile.CvPdfUrl_DE ?? existingProfile.CvPdfUrl_DE;

            _context.Entry(existingProfile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingProfile);
        }
    }
}
