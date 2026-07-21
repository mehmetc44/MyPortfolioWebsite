using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record GetProjectByIdQuery(string Id, string Lang) : IRequest<ProjectDto>;

    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public GetProjectByIdQueryHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();
            string cacheKey = $"Project_{request.Id}_{lang}";

            if (!_cache.TryGetValue(cacheKey, out ProjectDto? dto))
            {
                var p = await _context.Projects.FindAsync(new object[] { request.Id }, cancellationToken);
                if (p == null)
                {
                    throw new KeyNotFoundException("Proje bulunamadı.");
                }

                dto = p.MapToDto(lang);
                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(30));
            }

            return dto!;
        }
    }
}
