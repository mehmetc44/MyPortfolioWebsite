using Mapster;
using Server.Models;

namespace Server.CQRS.Projects
{
    public static class ProjectMappingConfig
    {
        public static ProjectDto MapToDto(this ProjectEntity p, string lang)
        {
            var dto = p.Adapt<ProjectDto>();
            dto.Title = lang == "en" ? p.Title_EN : (lang == "de" ? p.Title_DE : p.Title_TR);
            dto.SubTag = lang == "en" ? p.SubTag_EN : (lang == "de" ? p.SubTag_DE : p.SubTag_TR);
            dto.Description = lang == "en" ? p.Description_EN : (lang == "de" ? p.Description_DE : p.Description_TR);
            dto.DetailText = lang == "en" ? p.DetailText_EN : (lang == "de" ? p.DetailText_DE : p.DetailText_TR);
            return dto;
        }
    }
}
