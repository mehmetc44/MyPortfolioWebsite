using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Project : BaseEntity
    {
        // 1. KAPAK FOTOĞRAFI (Hızlı listeleme için burada durmalı)
        public string? CoverImageUrl { get; set; } 
        
        // 2. GALERİ (Detay sayfası için ilişkili tablo)
        public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();

        public MultiLanguageString Title { get; set; } = new();
        public MultiLanguageString Description { get; set; } = new();
        
        public Guid CategoryId { get; set; } // Foreign Key
        public Category Category { get; set; } = null!; // Navigation Property
        public string? Client { get; set; }
        public DateTime Date { get; set; }
    }
}