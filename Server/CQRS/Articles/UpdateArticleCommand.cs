using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record UpdateArticleCommand(string Id, ArticleEntity UpdatedArticle) : IRequest<ArticleEntity>;

    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, ArticleEntity>
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _cache;

        public UpdateArticleCommandHandler(AppDbContext context, IFileService fileService, IMemoryCache cache)
        {
            _context = context;
            _fileService = fileService;
            _cache = cache;
        }

        public async Task<ArticleEntity> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var updatedArticle = request.UpdatedArticle;

            if (id != updatedArticle.Id)
            {
                throw new ArgumentException("ID uyuşmazlığı.");
            }

            var existing = await _context.Articles.FindAsync(new object[] { id }, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException("Güncellenecek makale bulunamadı.");
            }

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

            var newImages = ExtractImageUrls(updatedArticle.DetailText_TR, updatedArticle.DetailText_EN, updatedArticle.DetailText_DE, updatedArticle.ImageUrl);

            var removedImages = oldImages.Except(newImages, StringComparer.OrdinalIgnoreCase).ToList();
            foreach (var imgUrl in removedImages)
            {
                try
                {
                    await _fileService.DeleteFileAsync(imgUrl);
                }
                catch
                {
                    // Fail silently
                }
            }

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);

            ClearArticlesCache(id);

            return existing;
        }

        private void ClearArticlesCache(string id)
        {
            _cache.Remove("Articles_tr");
            _cache.Remove("Articles_en");
            _cache.Remove("Articles_de");
            _cache.Remove($"Article_{id}_tr");
            _cache.Remove($"Article_{id}_en");
            _cache.Remove($"Article_{id}_de");
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
    }
}
