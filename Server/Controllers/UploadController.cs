using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using System;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // POST: api/upload/avatar
        [HttpPost("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar(IFormFile file, [FromQuery] string? oldUrl = null)
        {
            try
            {
                var relativePath = await _fileService.SaveFileAsync(file, "avatars", oldUrl);
                return Ok(new { url = relativePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/upload/cv
        [HttpPost("cv")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCv(IFormFile file, [FromQuery] string? oldUrl = null)
        {
            try
            {
                var relativePath = await _fileService.SaveFileAsync(file, "cvs", oldUrl);
                return Ok(new { url = relativePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
