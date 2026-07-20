using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Server.Services
{
    public class SupabaseStorageService : IFileService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseKey;
        private readonly string _bucket;

        public SupabaseStorageService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Retrieve credentials from environment variables or appsettings configuration
            _supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") 
                           ?? configuration["SUPABASE_URL"] 
                           ?? configuration["Supabase:Url"] 
                           ?? "";

            _supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY")
                           ?? Environment.GetEnvironmentVariable("SUPABASE_KEY")
                           ?? configuration["SUPABASE_KEY"]
                           ?? configuration["Supabase:Key"]
                           ?? "";

            _bucket = Environment.GetEnvironmentVariable("SUPABASE_BUCKET")
                      ?? configuration["SUPABASE_BUCKET"]
                      ?? configuration["Supabase:Bucket"]
                      ?? "portfolio";

            _supabaseUrl = _supabaseUrl.TrimEnd('/');
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder, string? oldUrl = null)
        {
            return await UploadFileAsync(file, subFolder, oldUrl);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string subFolder, string? oldUrl = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Yüklenen dosya geçersiz veya boş.");
            }

            // Remove old file if specified
            if (!string.IsNullOrEmpty(oldUrl))
            {
                await DeleteFileAsync(oldUrl);
            }

            // Sanitize filename
            var originalFileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            var cleanName = RemoveTurkishAndSpecialChars(nameWithoutExt);

            var uniqueFileName = $"{Guid.NewGuid()}_{cleanName}{extension}";
            var objectPath = string.IsNullOrWhiteSpace(subFolder) 
                ? uniqueFileName 
                : $"{subFolder.Trim('/')}/{uniqueFileName}";

            // If Supabase URL or Key is missing, log warning and fallback to relative pathing
            if (string.IsNullOrWhiteSpace(_supabaseUrl) || string.IsNullOrWhiteSpace(_supabaseKey))
            {
                Console.WriteLine($"[SupabaseStorage] Warning: SUPABASE_URL or SUPABASE_KEY missing. Returning relative path: storage/{objectPath}");
                return GetPublicUrl(objectPath);
            }

            // Supabase REST Upload Endpoint: POST /storage/v1/object/{bucket}/{objectPath}
            var requestUrl = $"{_supabaseUrl}/storage/v1/object/{_bucket}/{objectPath}";

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            AddSupabaseHeaders(request);
            request.Headers.Add("x-upsert", "true");

            using var stream = file.OpenReadStream();
            using var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errBody = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"[SupabaseStorage] Upload HTTP {response.StatusCode}: {errBody}");
            }

            return GetPublicUrl(objectPath);
        }

        public bool DeleteFile(string? relativeUrl)
        {
            return DeleteFileAsync(relativeUrl).GetAwaiter().GetResult();
        }

        public async Task<bool> DeleteFileAsync(string? relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return false;

            try
            {
                var objectPath = ExtractObjectPath(relativeUrl);
                if (string.IsNullOrWhiteSpace(objectPath)) return false;

                if (string.IsNullOrWhiteSpace(_supabaseUrl) || string.IsNullOrWhiteSpace(_supabaseKey))
                {
                    return false;
                }

                // Supabase Storage DELETE API: DELETE /storage/v1/object/{bucket}/{objectPath}
                var requestUrl = $"{_supabaseUrl}/storage/v1/object/{_bucket}/{objectPath}";

                using var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
                AddSupabaseHeaders(request);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SupabaseStorage] DeleteFileAsync error: {ex.Message}");
                return false;
            }
        }

        public bool DeleteDirectory(string? relativeSubFolder)
        {
            return DeleteDirectoryAsync(relativeSubFolder).GetAwaiter().GetResult();
        }

        public async Task<bool> DeleteDirectoryAsync(string? relativeSubFolder)
        {
            if (string.IsNullOrWhiteSpace(relativeSubFolder)) return false;

            try
            {
                var prefix = ExtractObjectPath(relativeSubFolder);
                if (string.IsNullOrWhiteSpace(prefix)) return false;

                if (string.IsNullOrWhiteSpace(_supabaseUrl) || string.IsNullOrWhiteSpace(_supabaseKey))
                {
                    return false;
                }

                // 1. List objects matching the folder prefix
                var listUrl = $"{_supabaseUrl}/storage/v1/object/list/{_bucket}";
                using var listRequest = new HttpRequestMessage(HttpMethod.Post, listUrl);
                AddSupabaseHeaders(listRequest);

                var bodyJson = JsonSerializer.Serialize(new { prefix = prefix, limit = 100 });
                listRequest.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                var listResponse = await _httpClient.SendAsync(listRequest);
                if (!listResponse.IsSuccessStatusCode) return false;

                var jsonContent = await listResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                var prefixes = new List<string>();
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in root.EnumerateArray())
                    {
                        if (item.TryGetProperty("name", out var nameProp))
                        {
                            prefixes.Add($"{prefix.TrimEnd('/')}/{nameProp.GetString()}");
                        }
                    }
                }

                if (prefixes.Count == 0) return true;

                // 2. Bulk delete objects
                var deleteUrl = $"{_supabaseUrl}/storage/v1/object/{_bucket}";
                using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, deleteUrl);
                AddSupabaseHeaders(deleteRequest);

                var deleteBody = JsonSerializer.Serialize(new { prefixes = prefixes });
                deleteRequest.Content = new StringContent(deleteBody, Encoding.UTF8, "application/json");

                var deleteResponse = await _httpClient.SendAsync(deleteRequest);
                return deleteResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SupabaseStorage] DeleteDirectoryAsync error: {ex.Message}");
                return false;
            }
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

            var cleanPath = ExtractObjectPath(path);
            if (string.IsNullOrWhiteSpace(_supabaseUrl))
            {
                return $"storage/{cleanPath}";
            }

            return $"{_supabaseUrl}/storage/v1/object/public/{_bucket}/{cleanPath}";
        }

        private void AddSupabaseHeaders(HttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(_supabaseKey))
            {
                request.Headers.Add("apiKey", _supabaseKey);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
            }
        }

        private string ExtractObjectPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";

            string p = path;
            if (p.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                p.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                try { p = new Uri(p).AbsolutePath; } catch { }
            }

            p = p.TrimStart('/', '\\');

            // Strip public URL prefix if present
            var publicPrefix = $"storage/v1/object/public/{_bucket}/";
            if (p.StartsWith(publicPrefix, StringComparison.OrdinalIgnoreCase))
            {
                p = p.Substring(publicPrefix.Length);
            }

            // Strip "storage/" if present
            if (p.StartsWith("storage/", StringComparison.OrdinalIgnoreCase))
            {
                p = p.Substring(8);
            }

            // Strip bucket name if present
            if (p.StartsWith($"{_bucket}/", StringComparison.OrdinalIgnoreCase))
            {
                p = p.Substring(_bucket.Length + 1);
            }

            return p.TrimStart('/');
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
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z0-9\-_]", "_");
            return text.Trim('_');
        }
    }
}
