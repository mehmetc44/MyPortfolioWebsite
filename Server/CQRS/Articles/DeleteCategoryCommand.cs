using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record DeleteCategoryCommand(string CategoryName) : IRequest<int>;

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, int>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public DeleteCategoryCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<int> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var catToRemove = request.CategoryName.Trim();
            var affected = 0;

            var articles = await _context.Articles.ToListAsync(cancellationToken);

            foreach (var art in articles)
            {
                var cats = art.Category_TR
                    .Split(',')
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c) && c != catToRemove)
                    .ToList();

                var newCategory = string.Join(",", cats);

                if (newCategory == art.Category_TR) continue;

                // Also clean EN/DE in same-order fashion
                var catsEN = art.Category_EN
                    .Split(',')
                    .Select(c => c.Trim())
                    .ToList();
                var catsDE = art.Category_DE
                    .Split(',')
                    .Select(c => c.Trim())
                    .ToList();

                // Find the index of the removed category and remove from EN/DE too
                var originalCats = art.Category_TR.Split(',').Select(c => c.Trim()).ToList();
                var removeIdx = originalCats.IndexOf(catToRemove);
                if (removeIdx >= 0)
                {
                    if (removeIdx < catsEN.Count) catsEN.RemoveAt(removeIdx);
                    if (removeIdx < catsDE.Count) catsDE.RemoveAt(removeIdx);
                }

                art.Category_TR = newCategory;
                art.Category_EN = string.Join(",", catsEN);
                art.Category_DE = string.Join(",", catsDE);
                affected++;
            }

            if (affected > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                // Clear all article caches
                _cache.Remove("Articles_tr");
                _cache.Remove("Articles_en");
                _cache.Remove("Articles_de");
                foreach (var art in articles)
                {
                    _cache.Remove($"Article_{art.Id}_tr");
                    _cache.Remove($"Article_{art.Id}_en");
                    _cache.Remove($"Article_{art.Id}_de");
                }
            }

            return affected;
        }
    }
}
