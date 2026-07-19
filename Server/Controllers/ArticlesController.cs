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
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
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
                ImageUrl = NormalizeRelativeUrl(a.ImageUrl),
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
                ImageUrl = NormalizeRelativeUrl(a.ImageUrl),
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

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
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
