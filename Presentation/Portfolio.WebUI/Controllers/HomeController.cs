using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.WebUI.Models.About;
using Portfolio.WebUI.Models.Portfolio;
using Portfolio.WebUI.Models.Resume;
using Portfolio.WebUI.Models.Models;
using System.Text.Json;
using Portfolio.WebUI.Services;
using Microsoft.AspNetCore.Localization; 
using Microsoft.AspNetCore.Http; 
namespace Portfolio.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ContentService _contentService;

        public HomeController(ILogger<HomeController> logger, ContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        public IActionResult Index()
        {
            var about = _contentService.GetAbout();
            var projects = _contentService.GetProjects();
            var projectCards = projects.Select(p => new ProjectCard
            {
                Id = p.Id,
                Title = p.Title,
                ShortDescription = p.ShortDescription,
                ThumbnailImage = p.ThumbnailImage,
                Category = p.Category
            }).ToList();
            var resume = _contentService.GetResume();

            return View(new PageViewModel(about, projectCards, resume));
        }

        // --- D�L DE���T�RME METODU ---
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (culture != null)
            {
                // �ereze yeni k�lt�r bilgisini kaydet (1 y�l ge�erli)
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddYears(1),
                        IsEssential = true // Taray�c�n�n �erezi silmesini engeller
                    }
                );
            }

            // Kullan�c�y� geldi�i sayfaya geri y�nlendir ve de�i�ikli�i uygula
            return LocalRedirect(returnUrl ?? "/");
        }
        // -----------------------------
    }
}