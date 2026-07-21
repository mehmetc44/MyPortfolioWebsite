using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Profile
{
    public record GetProfileQuery(string Lang) : IRequest<ProfileDto>;

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public GetProfileQueryHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();
            string cacheKey = $"Profile_{lang}";

            if (!_cache.TryGetValue(cacheKey, out ProfileDto? dto))
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(cancellationToken);
                if (profile == null)
                {
                    throw new KeyNotFoundException("Profil bulunamadı.");
                }

                dto = ProfileMappingConfig.MapToDto(profile, lang);
                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(30));
            }

            return dto!;
        }
    }
}
