using Mapster;
using Server.Models;

namespace Server.CQRS.Profile
{
    public static class ProfileMappingConfig
    {
        public static ProfileDto MapToDto(this ProfileEntity profile, string lang)
        {
            var dto = profile.Adapt<ProfileDto>();
            dto.Tag = lang == "en" ? profile.Tag_EN : (lang == "de" ? profile.Tag_DE : profile.Tag_TR);
            dto.Bio = lang == "en" ? profile.Bio_EN : (lang == "de" ? profile.Bio_DE : profile.Bio_TR);
            dto.CvText = lang == "en" ? profile.CvText_EN : (lang == "de" ? profile.CvText_DE : profile.CvText_TR);
            dto.CvPdfUrl = lang == "en" ? profile.CvPdfUrl_EN : (lang == "de" ? profile.CvPdfUrl_DE : profile.CvPdfUrl_TR);
            dto.Job = lang == "en" ? profile.Job_EN : (lang == "de" ? profile.Job_DE : profile.Job_TR);
            dto.Education = lang == "en" ? profile.Education_EN : (lang == "de" ? profile.Education_DE : profile.Education_TR);
            dto.Motto = lang == "en" ? profile.Motto_EN : (lang == "de" ? profile.Motto_DE : profile.Motto_TR);
            return dto;
        }
    }
}
