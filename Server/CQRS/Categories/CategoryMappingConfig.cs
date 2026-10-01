using Server.Models;

namespace Server.CQRS.Categories
{
    public static class CategoryMappingConfig
    {
        public static CategoryDto MapToDto(CategoryEntity c)
        {
            return new CategoryDto
            {
                Id = c.Id,
                Name_TR = c.Name_TR,
                Name_EN = c.Name_EN,
                Name_DE = c.Name_DE,
                IsSubCategory = c.IsSubCategory,
                ParentId = c.ParentId
            };
        }
    }
}
