using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record ReorderArticlesCommand(List<ArticleReorderItem> Items) : IRequest;

    public class ArticleReorderItem
    {
        public string Id { get; set; } = "";
        public int OrderIndex { get; set; }
    }

    public class ReorderArticlesCommandHandler : IRequestHandler<ReorderArticlesCommand>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public ReorderArticlesCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task Handle(ReorderArticlesCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || request.Items.Count == 0)
                return;

            foreach (var item in request.Items)
            {
                var article = await _context.Articles.FindAsync(new object[] { item.Id }, cancellationToken);
                if (article != null)
                {
                    article.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Clear articles lists caches
            _cache.Remove("Articles_tr");
            _cache.Remove("Articles_en");
            _cache.Remove("Articles_de");
        }
    }
}
