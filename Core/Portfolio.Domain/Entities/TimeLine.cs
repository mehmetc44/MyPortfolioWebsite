using Portfolio.Domain.Enums;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Timeline : BaseEntity
    {
        public TimelineType Type { get; set; } // Eğitim mi, İş mi?

        public string Period { get; set; } // "2020 - 2024" veya "2023 - Present"

        // Başlık / Ünvan (Örn: Senior Developer / Kıdemli Geliştirici)
        public MultiLanguageString Title { get; set; } = new();

        // Kurum / Okul Adı (Örn: Google / ODTÜ)
        public MultiLanguageString CompanyOrSchool { get; set; } = new();

        // Açıklama (Markdown formatında, madde işaretli vs.)
        public MultiLanguageString Description { get; set; } = new();
        
        // Opsiyonel: Sıralama yapmak istersen (En yeniyi en üste koymak için Tarih var ama manuel sıra da gerekebilir)
        public int Order { get; set; }
    }
}
