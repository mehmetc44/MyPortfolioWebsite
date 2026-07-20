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

            // Create target folders dynamically under Storage/{subFolder} with canonical absolute pathing
            var rootFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
            var uploadsFolder = Path.GetFullPath(Path.Combine(rootFolder, "Storage", subFolder));

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

            // Sanitize filename to avoid special characters, spaces, or URL encoding bugs
            var originalFileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            var cleanName = RemoveTurkishAndSpecialChars(nameWithoutExt);

            // Generate unique filename to avoid collision
            var uniqueFileName = $"{Guid.NewGuid()}_{cleanName}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative route
            return $"storage/{subFolder}/{uniqueFileName}";
        }

        private static string RemoveTurkishAndSpecialChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return "file";
            string[] unaccented = { "c", "C", "g", "G", "i", "I", "o", "O", "s", "S", "u", "U" };
            string[] turkish = { "ç", "Ç", "ğ", "Ğ", "ı", "İ", "ö", "Ö", "ş", "Ş", "ü", "Ü" };
            for (int i = 0; i < turkish.Length; i++)
            {
                text = text.Replace(turkish[i], unaccented[i]);
            }
            // Replace spaces and special non-alphanumeric chars with underscore
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z0-9\-_]", "_");
            return text.Trim('_');
        }
    }
}
