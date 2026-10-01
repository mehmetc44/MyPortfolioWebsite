using System.Collections.Generic;

namespace Server.CQRS.Categories
{
    public class CategoryDto
    {
        public string Id { get; set; } = "";
        public string Name_TR { get; set; } = "";
        public string Name_EN { get; set; } = "";
        public string Name_DE { get; set; } = "";
        public bool IsSubCategory { get; set; }
        public string? ParentId { get; set; }
        
        // For hierarchical presentation in UI
        public List<CategoryDto> SubCategories { get; set; } = new();
    }
}
