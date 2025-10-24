using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyPortfolio.Models.About;
using MyPortfolio.Models.Portfolio;
using MyPortfolio.Models.Resume;
using MyPortfolio.Models;
using System.Text.Json;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {   
        private readonly ILogger<HomeController> _logger;
        private readonly ContentService _contentService;

        public HomeController(ILogger<HomeController> logger, ContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        [HttpGet("")]
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
    }
}