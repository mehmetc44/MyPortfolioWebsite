using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.DTOs.PersonalInfo;
using Portfolio.WebUI.Models.PersonalInfo;

namespace Portfolio.WebUI.Controllers
{
    public class AdminController : Controller
    {
        IPersonalInfoService _personalInfoService;
        private readonly ISiteImageFileService _siteImageService;
        //private readonly IResumeFileService _resumeService;
        //private readonly IProjectImageFileService _projectImageService;

        public AdminController(
            ISiteImageFileService siteImageService)
        {
            _siteImageService = siteImageService;
            //_resumeService = resumeService;
            //_projectImageService = projectImageService;
        }



        public async Task<ActionResult> Index()
        {
            var _personalInfoDto = await _personalInfoService.GetPersonalInfoAsync();
            var _profilePhoto = await _siteImageService.GetSiteImageFileByTypeAsync(Domain.Enums.SiteImageType.SiteProfileImage);
            var _heroPhoto = await _siteImageService.GetSiteImageFileByTypeAsync(Domain.Enums.SiteImageType.SiteHeroImage);
            var view = new PersonalInfoViewModel(){
                personalInfoDto = _personalInfoDto,
                profilePhotoFullPath = _profilePhoto.FullPath,
                heroPhotoFullPath = _heroPhoto.FullPath
            };
            return View(view);
        }
        public ActionResult Resume()
        {
            return View();
        }
        public ActionResult Portfolio()
        {
            return View();
        }
        public ActionResult References()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePersonalInfo(UpdatePersonalInfoDto dto)
        {
            await _personalInfoService.UpdatePersonalInfoAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
