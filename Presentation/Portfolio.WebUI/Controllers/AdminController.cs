using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.DTOs.PersonalInfo;

namespace Portfolio.WebUI.Controllers
{
    public class AdminController : Controller
    {
        IPersonalInfoService _personalInfoService;

        public AdminController(IPersonalInfoService personalInfoService)
        {
            _personalInfoService = personalInfoService;
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
        public async Task<IActionResult> UpdateAboutMe(UpdatePersonalInfoDto dto, IFormFile? heroPhoto, IFormFile? profilePhoto)
        {
            await _personalInfoService.UpdateAboutMeAsync(dto,heroPhoto,profilePhoto);
            return Ok();
        }
    }
}
