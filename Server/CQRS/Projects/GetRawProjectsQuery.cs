using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record GetRawProjectsQuery : IRequest<IEnumerable<ProjectEntity>>;

    public class GetRawProjectsQueryHandler : IRequestHandler<GetRawProjectsQuery, IEnumerable<ProjectEntity>>
    {
        private readonly AppDbContext _context;

        public GetRawProjectsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectEntity>> Handle(GetRawProjectsQuery request, CancellationToken cancellationToken)
        {
            var list = await _context.Projects.OrderBy(p => p.OrderIndex).ToListAsync(cancellationToken);
            return list;
        }
    }
}
