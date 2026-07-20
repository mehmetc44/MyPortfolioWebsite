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

        public bool DeleteFile(string? relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return false;

            try
            {
                // Strip domain prefix if present (e.g. http://localhost:5169/storage/projects/foo.png)
                string path = relativeUrl;
                if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    try { path = new Uri(path).AbsolutePath; } catch { }
                }

                // Clean leading slashes
                path = path.TrimStart('/', '\\');

                // If path starts with "storage/", strip it to get relative path under Storage directory
                if (path.StartsWith("storage/", StringComparison.OrdinalIgnoreCase) || 
                    path.StartsWith("storage\\", StringComparison.OrdinalIgnoreCase))
                {
                    path = path.Substring(8);
                }

                // If path points to assets/ or non-storage, skip deletion
                if (path.StartsWith("assets/", StringComparison.OrdinalIgnoreCase) || 
                    path.StartsWith("assets\\", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var rootFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
                var storageFolder = Path.GetFullPath(Path.Combine(rootFolder, "Storage"));
                var fullFilePath = Path.GetFullPath(Path.Combine(storageFolder, path));

                // Path traversal security check
                if (!fullFilePath.StartsWith(storageFolder, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                    return true;
                }
            }
            catch
            {
                // Fail silently
            }

            return false;
        }

        public bool DeleteDirectory(string? relativeSubFolder)
        {
            if (string.IsNullOrWhiteSpace(relativeSubFolder)) return false;

            try
            {
                string path = relativeSubFolder.TrimStart('/', '\\');
                if (path.StartsWith("storage/", StringComparison.OrdinalIgnoreCase) || 
                    path.StartsWith("storage\\", StringComparison.OrdinalIgnoreCase))
                {
                    path = path.Substring(8);
                }

                var rootFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));
                var storageFolder = Path.GetFullPath(Path.Combine(rootFolder, "Storage"));
                var fullDirPath = Path.GetFullPath(Path.Combine(storageFolder, path));

                if (!fullDirPath.StartsWith(storageFolder, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (Directory.Exists(fullDirPath))
                {
                    Directory.Delete(fullDirPath, recursive: true);
                    return true;
                }
            }
            catch
            {
                // Fail silently
            }
            return false;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subFolder, string? oldUrl = null)
        {
            return await SaveFileAsync(file, subFolder, oldUrl);
        }

        public Task<bool> DeleteFileAsync(string? relativeUrl)
        {
            return Task.FromResult(DeleteFile(relativeUrl));
        }

        public Task<bool> DeleteDirectoryAsync(string? relativeSubFolder)
        {
            return Task.FromResult(DeleteDirectory(relativeSubFolder));
        }

        public string GetPublicUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || 
                path.StartsWith("assets/", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }
            var clean = path.TrimStart('/', '\\');
            if (clean.StartsWith("storage/", StringComparison.OrdinalIgnoreCase))
            {
                clean = clean.Substring(8);
            }
            return $"storage/{clean}";
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
