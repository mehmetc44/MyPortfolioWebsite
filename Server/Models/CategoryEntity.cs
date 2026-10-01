using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class CategoryEntity
    {
        [Key]
        public string Id { get; set; } = ""; // Slug format (e.g. "ai", "machine-learning")
        
        public string Name_TR { get; set; } = "";
        public string Name_EN { get; set; } = "";
        public string Name_DE { get; set; } = "";
        
        public bool IsSubCategory { get; set; } = false;
        
        // If IsSubCategory is true, this holds the Id of the parent (Main Category)
        // If IsSubCategory is false, this is null
        public string? ParentId { get; set; }
    }
}
