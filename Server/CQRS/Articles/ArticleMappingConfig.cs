using Mapster;
using Server.Models;

namespace Server.CQRS.Articles
{
    public static class ArticleMappingConfig
    {
        public static ArticleDto MapToDto(this ArticleEntity a, string lang)
        {
            var dto = a.Adapt<ArticleDto>();
            dto.Title = lang == "en" ? a.Title_EN : (lang == "de" ? a.Title_DE : a.Title_TR);
            dto.SubTag = lang == "en" ? a.SubTag_EN : (lang == "de" ? a.SubTag_DE : a.SubTag_TR);
            dto.Excerpt = lang == "en" ? a.Excerpt_EN : (lang == "de" ? a.Excerpt_DE : a.Excerpt_TR);
            dto.DetailText = lang == "en" ? a.DetailText_EN : (lang == "de" ? a.DetailText_DE : a.DetailText_TR);
            return dto;
        }
    }
}
