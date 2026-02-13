using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Testimonial : BaseEntity
    {
        public string ClientName { get; set; } = null!;
        
        public string? ClientTitle { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public int Rating { get; set; }
        public MultiLanguageString Comment { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }
}
