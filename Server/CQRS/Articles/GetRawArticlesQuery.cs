using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Articles
{
    public record GetRawArticlesQuery : IRequest<IEnumerable<ArticleEntity>>;

    public class GetRawArticlesQueryHandler : IRequestHandler<GetRawArticlesQuery, IEnumerable<ArticleEntity>>
    {
        private readonly AppDbContext _context;

        public GetRawArticlesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArticleEntity>> Handle(GetRawArticlesQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.Articles.OrderBy(a => a.OrderIndex).ToListAsync(cancellationToken);
            return list;
        }
    }
}
