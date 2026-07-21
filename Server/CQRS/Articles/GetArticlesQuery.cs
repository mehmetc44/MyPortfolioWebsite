using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record GetArticlesQuery(string Lang) : IRequest<IEnumerable<ArticleDto>>;

    public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, IEnumerable<ArticleDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public GetArticlesQueryHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<ArticleDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();
            string cacheKey = $"Articles_{lang}";

            if (!_cache.TryGetValue(cacheKey, out List<ArticleDto>? list))
            {
                var dbList = await _context.Articles.OrderBy(a => a.OrderIndex).ToListAsync(cancellationToken);
                list = dbList.Select(a => a.MapToDto(lang)).ToList();

                _cache.Set(cacheKey, list, TimeSpan.FromMinutes(30));
            }

            return list!;
        }
    }
}
