using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.WebUI.Models.Portfolio;
using Portfolio.WebUI.Services;

namespace Portfolio.WebUI.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly ContentService _contentService;

        public ProjectController(ILogger<ProjectController> logger, ContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        public IActionResult Details()
        {
            var projectDetails = new ProjectDetail
            {
                Id = 12,
                Title = "Proje1",
                LongDescription = "Proje Açıklama",
                GalleryImages = new List<string>{"books-1.jpg","books-2.jpg","books-3.jpg"},
                ProjectDate = "22.11.1963",
                Client = "Müşteri",
                ProjectUrl = "project.ProjectUrl",
                Category = "project.Category"
            };
            return View(projectDetails);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}