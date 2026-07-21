using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record GetProjectsQuery(string Lang) : IRequest<IEnumerable<ProjectDto>>;

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public GetProjectsQueryHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();
            string cacheKey = $"Projects_{lang}";

            if (!_cache.TryGetValue(cacheKey, out List<ProjectDto>? list))
            {
                var dbList = await _context.Projects.OrderBy(p => p.OrderIndex).ToListAsync(cancellationToken);
                list = dbList.Select(p => p.MapToDto(lang)).ToList();

                _cache.Set(cacheKey, list, TimeSpan.FromMinutes(30));
            }

            return list!;
        }
    }
}
