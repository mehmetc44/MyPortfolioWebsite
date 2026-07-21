using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record DeleteArticleCommand(string Id) : IRequest;

    public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand>
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _cache;

        public DeleteArticleCommandHandler(AppDbContext context, IFileService fileService, IMemoryCache cache)
        {
            _context = context;
            _fileService = fileService;
            _cache = cache;
        }

        public async Task Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var article = await _context.Articles.FindAsync(new object[] { id }, cancellationToken);
            if (article == null)
            {
                throw new KeyNotFoundException("Silinecek makale bulunamadı.");
            }

            var imagesToDelete = ExtractImageUrls(article.DetailText_TR, article.DetailText_EN, article.DetailText_DE, article.ImageUrl);
            foreach (var imgUrl in imagesToDelete)
            {
                try { await _fileService.DeleteFileAsync(imgUrl); } catch { }
            }
            try { await _fileService.DeleteDirectoryAsync($"blog/{id}"); } catch { }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync(cancellationToken);

            ClearArticlesCache(id);
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
