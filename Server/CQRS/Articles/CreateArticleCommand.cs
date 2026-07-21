using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record CreateArticleCommand(ArticleEntity Article) : IRequest<ArticleEntity>;

    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, ArticleEntity>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public CreateArticleCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ArticleEntity> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var article = request.Article;
            if (string.IsNullOrWhiteSpace(article.Id))
            {
                throw new ArgumentException("Makale ID'si boş olamaz.");
            }

            var exists = await _context.Articles.AnyAsync(a => a.Id == article.Id, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException("Bu ID'ye sahip bir makale zaten mevcut.");
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync(cancellationToken);

            ClearArticlesCache(article.Id);

            return article;
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
    }
}
