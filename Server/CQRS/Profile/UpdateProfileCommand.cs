using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Profile
{
    public record UpdateProfileCommand(ProfileEntity UpdatedProfile) : IRequest<ProfileEntity>;

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileEntity>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public UpdateProfileCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProfileEntity> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var existingProfile = await _context.Profiles.FirstOrDefaultAsync(cancellationToken);
            if (existingProfile == null)
            {
                throw new KeyNotFoundException("Güncellenecek profil bulunamadı.");
            }

            var updatedProfile = request.UpdatedProfile;

            // Map values with fallback to existing values to avoid null insertions
            existingProfile.Name = updatedProfile.Name ?? existingProfile.Name;
            existingProfile.Tag_TR = updatedProfile.Tag_TR ?? existingProfile.Tag_TR;
            existingProfile.Tag_EN = updatedProfile.Tag_EN ?? existingProfile.Tag_EN;
            existingProfile.Tag_DE = updatedProfile.Tag_DE ?? existingProfile.Tag_DE;
            existingProfile.Bio_TR = updatedProfile.Bio_TR ?? existingProfile.Bio_TR;
            existingProfile.Bio_EN = updatedProfile.Bio_EN ?? existingProfile.Bio_EN;
            existingProfile.Bio_DE = updatedProfile.Bio_DE ?? existingProfile.Bio_DE;
            existingProfile.AvatarUrl = updatedProfile.AvatarUrl ?? existingProfile.AvatarUrl;
            existingProfile.Repos = updatedProfile.Repos;
            existingProfile.Pubs = updatedProfile.Pubs;
            existingProfile.Github = updatedProfile.Github ?? existingProfile.Github;
            existingProfile.Linkedin = updatedProfile.Linkedin ?? existingProfile.Linkedin;
            existingProfile.Instagram = updatedProfile.Instagram ?? existingProfile.Instagram;
            existingProfile.Medium = updatedProfile.Medium ?? existingProfile.Medium;
            existingProfile.CvText_TR = updatedProfile.CvText_TR ?? existingProfile.CvText_TR;
            existingProfile.CvText_EN = updatedProfile.CvText_EN ?? existingProfile.CvText_EN;
            existingProfile.CvText_DE = updatedProfile.CvText_DE ?? existingProfile.CvText_DE;
            existingProfile.CvPdfUrl_TR = updatedProfile.CvPdfUrl_TR ?? existingProfile.CvPdfUrl_TR;
            existingProfile.CvPdfUrl_EN = updatedProfile.CvPdfUrl_EN ?? existingProfile.CvPdfUrl_EN;
            existingProfile.CvPdfUrl_DE = updatedProfile.CvPdfUrl_DE ?? existingProfile.CvPdfUrl_DE;

            // New fields mapping
            existingProfile.Job_TR = updatedProfile.Job_TR ?? existingProfile.Job_TR;
            existingProfile.Job_EN = updatedProfile.Job_EN ?? existingProfile.Job_EN;
            existingProfile.Job_DE = updatedProfile.Job_DE ?? existingProfile.Job_DE;
            existingProfile.Education_TR = updatedProfile.Education_TR ?? existingProfile.Education_TR;
            existingProfile.Education_EN = updatedProfile.Education_EN ?? existingProfile.Education_EN;
            existingProfile.Education_DE = updatedProfile.Education_DE ?? existingProfile.Education_DE;
            existingProfile.Motto_TR = updatedProfile.Motto_TR ?? existingProfile.Motto_TR;
            existingProfile.Motto_EN = updatedProfile.Motto_EN ?? existingProfile.Motto_EN;
            existingProfile.Motto_DE = updatedProfile.Motto_DE ?? existingProfile.Motto_DE;
            existingProfile.IsOpenToOffers = updatedProfile.IsOpenToOffers;

            _context.Entry(existingProfile).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);

            // Clear profile cache keys
            _cache.Remove("Profile_tr");
            _cache.Remove("Profile_en");
            _cache.Remove("Profile_de");

            return existingProfile;
        }
    }
}
