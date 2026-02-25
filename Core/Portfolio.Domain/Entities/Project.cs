using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string? CoverImagePath { get; set; } 
        public ICollection<ProjectImageFile> Images { get; set; } = new List<ProjectImageFile>();

        public MultiLanguageString Title { get; set; } = new();
        public MultiLanguageString Description { get; set; } = new();
        
        public Guid CategoryId { get; set; } 
        public Category Category { get; set; } = null!; 
        public string? Client { get; set; }
        public DateTime Date { get; set; }
    }
}