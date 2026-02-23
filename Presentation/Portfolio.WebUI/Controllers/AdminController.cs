using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.Abstraction.Storage.Local;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Domain.Entities;
using Portfolio.Domain.ValueObjects;
using Portfolio.Infrastructure.Services.Storage.Local;

namespace Portfolio.WebUI.Controllers
{
    public class AdminController : Controller
    {
        IAboutMeService _aboutMeService;
        ILocalStorage _storageService;

        public AdminController(IAboutMeService aboutMeService, ILocalStorage storageService)
        {
            _aboutMeService = aboutMeService;
            _storageService = storageService;
        }



        public ActionResult Index()
        {
            return View();
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


        // ... (namespace ve class tanımlamaların)

        [HttpPost]
        public async Task<IActionResult> UpdateAboutMe(UpdateAboutMeDto dto, IFormFile? heroPhoto, IFormFile? profilePhoto)
        {
            if (heroPhoto != null && heroPhoto.Length > 0)
            {
                FormFileCollection heroCollection = new FormFileCollection { heroPhoto };
                var uploadedHero = await _storageService.UploadAsync("img", heroCollection);
                dto.HeroImageUrl = uploadedHero.First().pathOrContainerName;
            }
            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                FormFileCollection profileCollection = new FormFileCollection { profilePhoto };
                var uploadedProfile = await _storageService.UploadAsync("img", profileCollection);
                dto.ImageUrl = uploadedProfile.First().pathOrContainerName;
            }
            await _aboutMeService.UpdateAboutMeAsync(dto);
            return RedirectToAction("Index");
        }
    }
}
