using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string? ImageUrl { get; set; }
        
        public MultiLanguageString Title { get; set; } = new();
        public MultiLanguageString Description { get; set; } = new();
        
        public string Category { get; set; } =null!;
        public string? Client { get; set; }
        public DateTime Date { get; set; }
    }
}