using Microsoft.AspNetCore.Mvc;
using Portfolio.WebUI.Models.Portfolio;


namespace Portfolio.WebUI.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;

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
    }
}