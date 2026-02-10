using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class AboutMe : BaseEntity
    {
        // --- Profil Görseli ---
        public string? ImageUrl { get; set; }
        public string? CvPdfUrl { get; set; } // "CV İndir" butonu için dosya yolu

        // --- Metin Alanları (Çok Dilli & Markdown) ---
        
        // Hero Bölümü (Kısa giriş: "Merhaba ben Mehmet, Yazılım Geliştiriciyim...")
        public MultiLanguageString Introduction { get; set; } = new(); 
        
        // Hakkımda Bölümü (Uzun biyografi)
        public MultiLanguageString Biography { get; set; } = new();

        // --- İletişim Bilgileri (Genelde çevrilmez) ---
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; } // Eğer adresi de çevirmek istersen MultiLanguageString yapabilirsin
        public string? MapFrameUrl { get; set; } // Google Maps Embed linki

        // --- Sosyal Medya ---
        public string? Facebook { get; set; }
        public string? Twitter { get; set; } // X
        public string? LinkedIn { get; set; }
        public string? Github { get; set; }
        public string? Instagram { get; set; }
        public string? Medium { get; set; }
    }
}