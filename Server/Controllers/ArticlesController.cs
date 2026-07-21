using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public ArticlesController(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/articles/raw
        [HttpGet("raw")]
        public async Task<IActionResult> GetRawArticles()
        {
            var list = await _context.Articles.OrderBy(a => a.OrderIndex).ToListAsync();
            return Ok(list);
        }

        // GET: api/articles?lang=tr
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] string lang = "tr")
        {
            var list = await _context.Articles.OrderBy(a => a.OrderIndex).ToListAsync();
            lang = lang.ToLower();

            var mappedList = list.Select(a => new
            {
                Id = a.Id,
                Title = lang == "en" ? a.Title_EN : (lang == "de" ? a.Title_DE : a.Title_TR),
                Category = a.Category,
                Date = a.Date,
                ReadTime = a.ReadTime,
                SubTag = lang == "en" ? a.SubTag_EN : (lang == "de" ? a.SubTag_DE : a.SubTag_TR),
                Excerpt = lang == "en" ? a.Excerpt_EN : (lang == "de" ? a.Excerpt_DE : a.Excerpt_TR),
                ImageUrl = a.ImageUrl,
                DetailText = lang == "en" ? a.DetailText_EN : (lang == "de" ? a.DetailText_DE : a.DetailText_TR)
            });

            return Ok(mappedList);
        }

        // GET: api/articles/saas-multitenancy?lang=tr
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(string id, [FromQuery] string lang = "tr")
        {
            var a = await _context.Articles.FindAsync(id);
            if (a == null)
            {
                return NotFound("Makale bulunamadı.");
            }

            lang = lang.ToLower();
            var mapped = new
            {
                Id = a.Id,
                Title = lang == "en" ? a.Title_EN : (lang == "de" ? a.Title_DE : a.Title_TR),
                Category = a.Category,
                Date = a.Date,
                ReadTime = a.ReadTime,
                SubTag = lang == "en" ? a.SubTag_EN : (lang == "de" ? a.SubTag_DE : a.SubTag_TR),
                Excerpt = lang == "en" ? a.Excerpt_EN : (lang == "de" ? a.Excerpt_DE : a.Excerpt_TR),
                ImageUrl = a.ImageUrl,
                DetailText = lang == "en" ? a.DetailText_EN : (lang == "de" ? a.DetailText_DE : a.DetailText_TR)
            };

            return Ok(mapped);
        }

        // POST: api/articles
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleEntity article)
        {
            if (string.IsNullOrWhiteSpace(article.Id))
            {
                return BadRequest("Makale ID'si boş olamaz.");
            }

            var exists = await _context.Articles.AnyAsync(a => a.Id == article.Id);
            if (exists)
            {
                return Conflict("Bu ID'ye sahip bir makale zaten mevcut.");
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        // PUT: api/articles/saas-multitenancy
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(string id, [FromBody] ArticleEntity updatedArticle)
        {
            if (id != updatedArticle.Id)
            {
                return BadRequest("ID uyuşmazlığı.");
            }

            var existing = await _context.Articles.FindAsync(id);
            if (existing == null)
            {
                return NotFound("Güncellenecek makale bulunamadı.");
            }

            // Track images previously present in old version
            var oldImages = ExtractImageUrls(existing.DetailText_TR, existing.DetailText_EN, existing.DetailText_DE, existing.ImageUrl);

            // Map values
            existing.Title_TR = updatedArticle.Title_TR;
            existing.Title_EN = updatedArticle.Title_EN;
            existing.Title_DE = updatedArticle.Title_DE;
            existing.Category = updatedArticle.Category;
            existing.Date = updatedArticle.Date;
            existing.ReadTime = updatedArticle.ReadTime;
            existing.SubTag_TR = updatedArticle.SubTag_TR;
            existing.SubTag_EN = updatedArticle.SubTag_EN;
            existing.SubTag_DE = updatedArticle.SubTag_DE;
            existing.Excerpt_TR = updatedArticle.Excerpt_TR;
            existing.Excerpt_EN = updatedArticle.Excerpt_EN;
            existing.Excerpt_DE = updatedArticle.Excerpt_DE;
            existing.ImageUrl = updatedArticle.ImageUrl;
            existing.DetailText_TR = updatedArticle.DetailText_TR;
            existing.DetailText_EN = updatedArticle.DetailText_EN;
            existing.DetailText_DE = updatedArticle.DetailText_DE;

            // Track images present in new updated version
            var newImages = ExtractImageUrls(updatedArticle.DetailText_TR, updatedArticle.DetailText_EN, updatedArticle.DetailText_DE, updatedArticle.ImageUrl);

            // Automatically clean up images removed during edit
            var removedImages = oldImages.Except(newImages, StringComparer.OrdinalIgnoreCase).ToList();
            foreach (var imgUrl in removedImages)
            {
                try
                {
                    await _fileService.DeleteFileAsync(imgUrl);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ArticlesController] Failed to delete unused image '{imgUrl}': {ex.Message}");
                }
            }

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // PUT: api/articles/reorder
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderArticles([FromBody] List<ArticleReorderItem> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest("Geçersiz sıralama verisi.");

            foreach (var item in items)
            {
                var article = await _context.Articles.FindAsync(item.Id);
                if (article != null)
                {
                    article.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/articles/saas-multitenancy
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound("Silinecek makale bulunamadı.");
            }

            // Automatically delete all associated images & folder from storage
            var imagesToDelete = ExtractImageUrls(article.DetailText_TR, article.DetailText_EN, article.DetailText_DE, article.ImageUrl);
            foreach (var imgUrl in imagesToDelete)
            {
                try { await _fileService.DeleteFileAsync(imgUrl); } catch { }
            }
            try { await _fileService.DeleteDirectoryAsync($"blog/{id}"); } catch { }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static HashSet<string> ExtractImageUrls(params string?[] contents)
        {
            var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (contents == null) return urls;

            var htmlRegex = new Regex(@"<img[^>]+src=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            var mdRegex = new Regex(@"!\[.*?\]\(\s*<?([^\s""'>\)]+)", RegexOptions.IgnoreCase);

            foreach (var content in contents)
            {
                if (string.IsNullOrWhiteSpace(content)) continue;
                
                // If content is direct image URL
                if (content.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    content.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || 
                    content.StartsWith("storage/", StringComparison.OrdinalIgnoreCase))
                {
                    urls.Add(content.Trim());
                }

                var htmlMatches = htmlRegex.Matches(content);
                foreach (Match m in htmlMatches)
                {
                    if (m.Groups.Count > 1)
                    {
                        var src = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(src))
                        {
                            urls.Add(src);
                        }
                    }
                }

                var mdMatches = mdRegex.Matches(content);
                foreach (Match m in mdMatches)
                {
                    if (m.Groups.Count > 1)
                    {
                        var src = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(src))
                        {
                            urls.Add(src);
                        }
                    }
                }
            }
            return urls;
        }

        // POST: api/articles/{id}/publish-medium
        [HttpPost("{id}/publish-medium")]
        public async Task<IActionResult> PublishToMedium(string id, [FromQuery] string? token = null)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound(new { message = "Makale bulunamadı." });
            }

            var mediumToken = !string.IsNullOrWhiteSpace(token)
                ? token
                : (Environment.GetEnvironmentVariable("MEDIUM_INTEGRATION_TOKEN") 
                   ?? Environment.GetEnvironmentVariable("MEDIUM_TOKEN"));

            if (string.IsNullOrWhiteSpace(mediumToken))
            {
                return BadRequest(new { message = "Medium Integration Token bulunamadı. Lütfen token girin veya .env dosyasına MEDIUM_INTEGRATION_TOKEN ekleyin." });
            }

            try
            {
                using var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", mediumToken);

                // 1. Get Medium User Info
                var meRes = await httpClient.GetAsync("https://api.medium.com/v1/me");
                if (!meRes.IsSuccessStatusCode)
                {
                    var meErr = await meRes.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Medium Kullanıcı Bilgisi Alınamadı ({meRes.StatusCode}): {meErr}" });
                }

                var meJson = await meRes.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(meJson);
                var userId = doc.RootElement.GetProperty("data").GetProperty("id").GetString();

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { message = "Medium Kullanıcı ID'si okunamadı." });
                }

                // 2. Prepare Post Payload
                var title = !string.IsNullOrWhiteSpace(article.Title_TR) ? article.Title_TR : (!string.IsNullOrWhiteSpace(article.Title_EN) ? article.Title_EN : article.Title_DE);
                var excerpt = !string.IsNullOrWhiteSpace(article.Excerpt_TR) ? article.Excerpt_TR : (!string.IsNullOrWhiteSpace(article.Excerpt_EN) ? article.Excerpt_EN : article.Excerpt_DE);
                var detail = !string.IsNullOrWhiteSpace(article.DetailText_TR) ? article.DetailText_TR : (!string.IsNullOrWhiteSpace(article.DetailText_EN) ? article.DetailText_EN : article.DetailText_DE);

                var htmlContent = $"<h1>{title}</h1>";
                if (!string.IsNullOrWhiteSpace(excerpt))
                {
                    htmlContent += $"<p><strong>{excerpt}</strong></p><hr/>";
                }
                htmlContent += detail;

                var categoryTag = !string.IsNullOrWhiteSpace(article.Category) ? article.Category.ToLower() : "technology";

                var postPayload = new
                {
                    title = title,
                    contentFormat = "html",
                    content = htmlContent,
                    tags = new[] { categoryTag, "software", "tech" },
                    publishStatus = "public"
                };

                var postJson = System.Text.Json.JsonSerializer.Serialize(postPayload);
                using var postContent = new System.Net.Http.StringContent(postJson, System.Text.Encoding.UTF8, "application/json");

                var postRes = await httpClient.PostAsync($"https://api.medium.com/v1/users/{userId}/posts", postContent);
                if (!postRes.IsSuccessStatusCode)
                {
                    var postErr = await postRes.Content.ReadAsStringAsync();
                    return BadRequest(new { message = $"Medium Paylaşım Hatası ({postRes.StatusCode}): {postErr}" });
                }

                var postResJson = await postRes.Content.ReadAsStringAsync();
                using var postDoc = System.Text.Json.JsonDocument.Parse(postResJson);
                var mediumUrl = postDoc.RootElement.GetProperty("data").GetProperty("url").GetString();

                return Ok(new { success = true, url = mediumUrl, message = "Makale Medium'da başarıyla yayınlandı!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Medium paylaşımı sırasında sunucu hatası: {ex.Message}" });
            }
        }

        /// <summary>
        /// Strips any absolute URL host prefix from a stored URL, returning only the relative path.
        /// </summary>
        private static string? NormalizeRelativeUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            if (!url.StartsWith("http")) return url;
            try
            {
                var uri = new System.Uri(url);
                return uri.AbsolutePath.TrimStart('/');
            }
            catch
            {
                return url;
            }
        }
    }

    public class ArticleReorderItem
    {
        public string Id { get; set; } = "";
        public int OrderIndex { get; set; }
    }
}
