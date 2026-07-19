using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using System;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
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
                var relativePath = await _fileService.SaveFileAsync(file, "avatar", oldUrl);
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
                var relativePath = await _fileService.SaveFileAsync(file, "cv", oldUrl);
                return Ok(new { url = relativePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/upload/project?projectId=xyz
        [HttpPost("project")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProjectImage(IFormFile file, [FromQuery] string projectId, [FromQuery] string? oldUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(projectId))
                {
                    return BadRequest(new { message = "projectId gereklidir." });
                }
                var cleanProjectId = projectId.Replace("..", "").Replace("/", "").Replace("\\", "");
                var relativePath = await _fileService.SaveFileAsync(file, $"projects/{cleanProjectId}", oldUrl);
                return Ok(new { url = relativePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/upload/article?articleId=xyz
        [HttpPost("article")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadArticleImage(IFormFile file, [FromQuery] string articleId, [FromQuery] string? oldUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(articleId))
                {
                    return BadRequest(new { message = "articleId gereklidir." });
                }
                var cleanArticleId = articleId.Replace("..", "").Replace("/", "").Replace("\\", "");
                var relativePath = await _fileService.SaveFileAsync(file, $"articles/{cleanArticleId}", oldUrl);
                return Ok(new { url = relativePath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
