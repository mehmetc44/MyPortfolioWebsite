using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.DTOs.File;

namespace Portfolio.WebUI.Controllers
{
    public class FileController : Controller
    {
        private readonly ISiteImageFileService _siteImageService;
        //private readonly IResumeFileService _resumeService;
        //private readonly IProjectImageFileService _projectImageService;

        public FileController(
            ISiteImageFileService siteImageService)
        {
            _siteImageService = siteImageService;
            //_resumeService = resumeService;
            //_projectImageService = projectImageService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile profilePhoto)
        {
            if (profilePhoto == null || profilePhoto.Length == 0)
            {
                return RedirectToAction("Index","Admin");
            }
            var dto = new SiteImageFileUploadDto
            {
                FileName = profilePhoto.FileName,
                Path = "img/site-images",
                SiteImageType = Domain.Enums.SiteImageType.SiteProfileImage
            };
            await _siteImageService.UploadAsync(dto, profilePhoto);
            return RedirectToAction("Index","Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHeroPhoto(IFormFile heroPhoto)
        {
            if (heroPhoto == null || heroPhoto.Length == 0)
            {
                return RedirectToAction("Index","Admin");
            }
            var dto = new SiteImageFileUploadDto
            {
                FileName = heroPhoto.FileName,
                Path = "img/site-images",
                SiteImageType = Domain.Enums.SiteImageType.SiteHeroImage
            };
            await _siteImageService.UploadAsync(dto, heroPhoto);
            return RedirectToAction("Index","Admin");
        }

        /*[HttpPost("upload-resume")]
        public async Task<IActionResult> UploadResume([FromForm] ResumeFileUploadDto dto, IFormFile file)
        {
            await _resumeService.UploadAsync(dto, file);
            return Ok(new { message = "CV başarıyla yüklendi." });
        }

        [HttpPost("upload-project-image")]
        public async Task<IActionResult> UploadProjectImage([FromForm] ProjectImageFileUploadDto dto, IFormFile file)
        {
            await _projectImageService.UploadAsync(dto, file);
            return Ok(new { message = "Proje görseli eklendi." });
        }

        [HttpDelete("delete-image/{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            await _siteImageService.DeleteAsync(id);
            return Ok();
        }*/
    }
}
