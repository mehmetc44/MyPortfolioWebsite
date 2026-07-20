using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Services.IFileService _fileService;

        public ProjectsController(AppDbContext context, Services.IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/projects/raw
        [HttpGet("raw")]
        public async Task<IActionResult> GetRawProjects()
        {
            var list = await _context.Projects.OrderBy(p => p.OrderIndex).ToListAsync();
            return Ok(list);
        }

        // GET: api/projects?lang=tr
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] string lang = "tr")
        {
            var list = await _context.Projects.OrderBy(p => p.OrderIndex).ToListAsync();
            lang = lang.ToLower();

            var mappedList = list.Select(p => new
            {
                Id = p.Id,
                Title = lang == "en" ? p.Title_EN : (lang == "de" ? p.Title_DE : p.Title_TR),
                Category = p.Category,
                Date = p.Date,
                Client = p.Client,
                SubTag = lang == "en" ? p.SubTag_EN : (lang == "de" ? p.SubTag_DE : p.SubTag_TR),
                Description = lang == "en" ? p.Description_EN : (lang == "de" ? p.Description_DE : p.Description_TR),
                Tech = p.Tech,
                RepoUrl = p.RepoUrl,
                DemoUrl = p.DemoUrl,
                ImagesJson = NormalizeImagesJson(p.ImagesJson),
                DetailText = lang == "en" ? p.DetailText_EN : (lang == "de" ? p.DetailText_DE : p.DetailText_TR)
            });

            return Ok(mappedList);
        }

        // GET: api/projects/platar-lpr?lang=tr
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(string id, [FromQuery] string lang = "tr")
        {
            var p = await _context.Projects.FindAsync(id);
            if (p == null)
            {
                return NotFound("Proje bulunamadı.");
            }

            lang = lang.ToLower();
            var mapped = new
            {
                Id = p.Id,
                Title = lang == "en" ? p.Title_EN : (lang == "de" ? p.Title_DE : p.Title_TR),
                Category = p.Category,
                Date = p.Date,
                Client = p.Client,
                SubTag = lang == "en" ? p.SubTag_EN : (lang == "de" ? p.SubTag_DE : p.SubTag_TR),
                Description = lang == "en" ? p.Description_EN : (lang == "de" ? p.Description_DE : p.Description_TR),
                Tech = p.Tech,
                RepoUrl = p.RepoUrl,
                DemoUrl = p.DemoUrl,
                ImagesJson = NormalizeImagesJson(p.ImagesJson),
                DetailText = lang == "en" ? p.DetailText_EN : (lang == "de" ? p.DetailText_DE : p.DetailText_TR)
            };

            return Ok(mapped);
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectEntity project)
        {
            if (string.IsNullOrWhiteSpace(project.Id))
            {
                return BadRequest("Proje ID'si boş olamaz.");
            }

            var exists = await _context.Projects.AnyAsync(p => p.Id == project.Id);
            if (exists)
            {
                return Conflict("Bu ID'ye sahip bir proje zaten mevcut.");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        // PUT: api/projects/platar-lpr
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, [FromBody] ProjectEntity updatedProject)
        {
            if (id != updatedProject.Id)
            {
                return BadRequest("ID uyuşmazlığı.");
            }

            var existing = await _context.Projects.FindAsync(id);
            if (existing == null)
            {
                return NotFound("Güncellenecek proje bulunamadı.");
            }

            // Delete any physical image files that were removed from the project
            DeleteRemovedPhysicalImages(existing.ImagesJson, updatedProject.ImagesJson);

            // Map values
            existing.Title_TR = updatedProject.Title_TR;
            existing.Title_EN = updatedProject.Title_EN;
            existing.Title_DE = updatedProject.Title_DE;
            existing.Category = updatedProject.Category;
            existing.Date = updatedProject.Date;
            existing.Client = updatedProject.Client;
            existing.SubTag_TR = updatedProject.SubTag_TR;
            existing.SubTag_EN = updatedProject.SubTag_EN;
            existing.SubTag_DE = updatedProject.SubTag_DE;
            existing.Description_TR = updatedProject.Description_TR;
            existing.Description_EN = updatedProject.Description_EN;
            existing.Description_DE = updatedProject.Description_DE;
            existing.Tech = updatedProject.Tech;
            existing.RepoUrl = updatedProject.RepoUrl;
            existing.DemoUrl = updatedProject.DemoUrl;
            existing.ImagesJson = updatedProject.ImagesJson;
            existing.DetailText_TR = updatedProject.DetailText_TR;
            existing.DetailText_EN = updatedProject.DetailText_EN;
            existing.DetailText_DE = updatedProject.DetailText_DE;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // PUT: api/projects/reorder
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderProjects([FromBody] List<ProjectReorderItem> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest("Geçersiz sıralama verisi.");

            foreach (var item in items)
            {
                var project = await _context.Projects.FindAsync(item.Id);
                if (project != null)
                {
                    project.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/projects/platar-lpr
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound("Silinecek proje bulunamadı.");
            }

            // Delete all associated project images and folder from disk
            DeleteRemovedPhysicalImages(project.ImagesJson, null);
            _fileService.DeleteDirectory($"projects/{id}");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void DeleteRemovedPhysicalImages(string? oldJson, string? newJson)
        {
            if (string.IsNullOrWhiteSpace(oldJson)) return;

            try
            {
                var oldList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(oldJson) ?? new List<string>();
                var newList = string.IsNullOrWhiteSpace(newJson) 
                    ? new List<string>() 
                    : (System.Text.Json.JsonSerializer.Deserialize<List<string>>(newJson) ?? new List<string>());

                // Find files present in old list but missing from new list
                var removedFiles = oldList.Where(oldImg => !newList.Contains(oldImg)).ToList();

                foreach (var imgPath in removedFiles)
                {
                    _fileService.DeleteFile(imgPath);
                }
            }
            catch
            {
                // Fail silently
            }
        }

        /// <summary>
        /// Strips any absolute URL host prefix from each image path in a JSON array string.
        /// e.g. ["http://localhost:5169/uploads/images/file.jpg"] => ["uploads/images/file.jpg"]
        /// </summary>
        private static string? NormalizeImagesJson(string? imagesJson)
        {
            if (string.IsNullOrEmpty(imagesJson)) return imagesJson;
            try
            {
                var urls = System.Text.Json.JsonSerializer.Deserialize<string[]>(imagesJson);
                if (urls == null) return imagesJson;
                var normalized = System.Array.ConvertAll(urls, url =>
                {
                    if (string.IsNullOrEmpty(url) || !url.StartsWith("http")) return url;
                    try { return new System.Uri(url).AbsolutePath.TrimStart('/'); }
                    catch { return url; }
                });
                return System.Text.Json.JsonSerializer.Serialize(normalized);
            }
            catch
            {
                return imagesJson;
            }
        }
    }

    public class ProjectReorderItem
    {
        public string Id { get; set; } = "";
        public int OrderIndex { get; set; }
    }
}
