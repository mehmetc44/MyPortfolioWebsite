using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MyPortfolio.Models;
using MyPortfolio.Models.About;
using MyPortfolio.Models.Portfolio;
using MyPortfolio.Models.Resume;

namespace MyPortfolio.Services
{
    public class ContentService
    {
        private readonly IWebHostEnvironment _env;

        public ContentService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public AboutModel GetAbout()
        {
            var filePath = Path.Combine(_env.WebRootPath, "data", "about.json");
            if (!File.Exists(filePath))
                return new AboutModel();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AboutModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public ResumeModel GetResume()
        {
            var filePath = Path.Combine(_env.WebRootPath, "data", "resume.json");
            if (!File.Exists(filePath))
                return new ResumeModel();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ResumeModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        public List<Project> GetProjects()
        {
            var filePath = Path.Combine(_env.WebRootPath, "data", "projects.json");
            if (!File.Exists(filePath))
                return new List<Project>();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Project>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Project>();
        }
    }
}
