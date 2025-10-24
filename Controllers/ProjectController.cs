using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyPortfolio.Models.Portfolio;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers
{
    [Route("[controller]")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly ContentService _contentService;

        public ProjectController(ILogger<ProjectController> logger, ContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            var project = _contentService.GetProjects().FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var projectDetails = new ProjectDetail
            {
                Id = project.Id,
                Title = project.Title,
                LongDescription = project.LongDescription,
                GalleryImages = new List<string>{
                project.ThumbnailImage?.ToString() ?? string.Empty
                }.Concat(project.GalleryImages ?? new List<string>()).ToList(),
                ProjectDate = project.ProjectDate,
                Client = project.Client,
                ProjectUrl = project.ProjectUrl,
                Category = project.Category
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