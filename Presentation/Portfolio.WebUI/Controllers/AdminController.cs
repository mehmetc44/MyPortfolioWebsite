using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.Abstraction.Storage.Local;
using Portfolio.Application.DTOs.AboutMe;
using Portfolio.Domain.ValueObjects;
using Portfolio.Infrastructure.Services.Storage.Local;

namespace Portfolio.WebUI.Controllers
{
    public class AdminController : Controller
    {
        IAboutMeService _aboutMeService;

        public AdminController(IAboutMeService aboutMeService)
        {
            _aboutMeService = aboutMeService;
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

        [HttpPost]
        public async Task<IActionResult> UpdateAboutMe(UpdateAboutMeDto dto, IFormFile? heroPhoto, IFormFile? profilePhoto)
        {
            await _aboutMeService.UpdateAboutMeAsync(dto,heroPhoto,profilePhoto);
            return Ok();
        }
    }
}
