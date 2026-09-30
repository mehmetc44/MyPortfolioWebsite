using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record RenameCategoryCommand(
        string OldName,
        string NewName,
        string NewName_EN,
        string NewName_DE
    ) : IRequest<int>;

    public class RenameCategoryCommandHandler : IRequestHandler<RenameCategoryCommand, int>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public RenameCategoryCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<int> Handle(RenameCategoryCommand request, CancellationToken cancellationToken)
        {
            var oldName = request.OldName.Trim();
            var newName = request.NewName.Trim();
            var newEN   = (request.NewName_EN ?? "").Trim();
            var newDE   = (request.NewName_DE ?? "").Trim();
            var affected = 0;

            var articles = await _context.Articles.ToListAsync(cancellationToken);

            foreach (var art in articles)
            {
                var trCats = art.Category_TR.Split(',').Select(c => c.Trim()).ToList();
                var enCats = art.Category_EN.Split(',').Select(c => c.Trim()).ToList();
                var deCats = art.Category_DE.Split(',').Select(c => c.Trim()).ToList();

                bool changed = false;
                for (int i = 0; i < trCats.Count; i++)
                {
                    if (trCats[i] != oldName) continue;

                    trCats[i] = newName;
                    if (i < enCats.Count) enCats[i] = newEN;
                    else enCats.Add(newEN);
                    if (i < deCats.Count) deCats[i] = newDE;
                    else deCats.Add(newDE);
                    changed = true;
                }

                if (!changed) continue;

                art.Category_TR = string.Join(",", trCats);
                art.Category_EN = string.Join(",", enCats);
                art.Category_DE = string.Join(",", deCats);
                affected++;
            }

            if (affected > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
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
