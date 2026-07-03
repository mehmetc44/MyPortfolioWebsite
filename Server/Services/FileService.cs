using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Server.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder, string? oldUrl = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Yüklenen dosya geçersiz veya boş.");
            }

            // Create target folders dynamically under wwwroot/uploads/{subFolder}
            var wwwRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(wwwRoot, "uploads", subFolder);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Delete old file if present
            if (!string.IsNullOrEmpty(oldUrl))
            {
                try
                {
                    // Extract filename from URL/path
                    var oldFileName = Path.GetFileName(oldUrl);
                    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }
                catch
                {
                    // Fail silently to prevent upload block if deletion fails
                }
            }

            // Generate unique filename to avoid collision
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative route
            return $"uploads/{subFolder}/{uniqueFileName}";
        }
    }
}
