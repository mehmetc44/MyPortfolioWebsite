using Mapster;
using Server.Models;
using System.Linq;

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

            // Çoklu kategori: virgülle ayrılmış TR kategorilerini dil bazlı çeviri ile eşleştir
            if (lang == "tr" || string.IsNullOrWhiteSpace(a.Category_TR))
            {
                dto.Category = a.Category_TR;
            }
            else
            {
                var trCats = a.Category_TR.Split(',').Select(c => c.Trim()).ToArray();
                var translations = lang == "en"
                    ? (a.Category_EN ?? "").Split(',').Select(c => c.Trim()).ToArray()
                    : (a.Category_DE ?? "").Split(',').Select(c => c.Trim()).ToArray();

                var validCats = new System.Collections.Generic.List<string>();
                for (int i = 0; i < trCats.Length; i++)
                {
                    if (string.IsNullOrEmpty(trCats[i])) continue;
                    var t = i < translations.Length ? translations[i] : "";
                    validCats.Add(string.IsNullOrWhiteSpace(t) ? trCats[i] : t);
                }
                dto.Category = string.Join(",", validCats);
            }

            dto.IsDraft = a.IsDraft;
            return dto;
        }
    }
}
