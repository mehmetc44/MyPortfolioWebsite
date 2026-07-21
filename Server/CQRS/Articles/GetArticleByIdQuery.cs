using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record GetArticleByIdQuery(string Id, string Lang) : IRequest<ArticleDto>;

    public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public GetArticleByIdQueryHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();
            string cacheKey = $"Article_{request.Id}_{lang}";

            if (!_cache.TryGetValue(cacheKey, out ArticleDto? dto))
            {
                var a = await _context.Articles.FindAsync(new object[] { request.Id }, cancellationToken);
                if (a == null)
                {
                    throw new KeyNotFoundException("Makale bulunamadı.");
                }

                dto = a.MapToDto(lang);
                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(30));
            }

            return dto!;
        }
    }
}
