using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Timeline : BaseEntity
    {
        public TimelineType Type { get; set; }
        public string Period { get; set; } = null!;
        public MultiLanguageString Title { get; set; } = new();
        public MultiLanguageString CompanyOrSchool { get; set; } = new();
        public MultiLanguageString Description { get; set; } = new();
        
        // Opsiyonel: Sıralama yapmak istersen (En yeniyi en üste koymak için Tarih var ama manuel sıra da gerekebilir)
        public int Order { get; set; }
    }
}
