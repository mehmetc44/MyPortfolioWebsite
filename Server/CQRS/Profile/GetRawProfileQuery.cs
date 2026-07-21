using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Profile
{
    public record GetRawProfileQuery : IRequest<ProfileEntity>;

    public class GetRawProfileQueryHandler : IRequestHandler<GetRawProfileQuery, ProfileEntity>
    {
        private readonly AppDbContext _context;

        public GetRawProfileQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileEntity> Handle(GetRawProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(cancellationToken);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profil bulunamadı.");
            }
            return profile;
        }
    }
}
