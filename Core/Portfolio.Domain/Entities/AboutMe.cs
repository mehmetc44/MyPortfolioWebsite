using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class AboutMe : BaseEntity
    {
        // --- Profil Görseli ---
        // Profil resmi genelde evrenseldir, dile göre değişmez.
        public string? ImageUrl { get; set; }
        
        // --- KRİTİK GÜNCELLEME: CV DOSYA YOLU ---
        // String yerine MultiLanguageString yaptık.
        // Böylece 3 farklı PDF yükleyip, dile göre sunabilirsin.
        public MultiLanguageString CvPath { get; set; } = new(); 

        // --- Metin Alanları (Çok Dilli) ---
        public MultiLanguageString Introduction { get; set; } = new(); // Hero (Kısa)
        public MultiLanguageString Biography { get; set; } = new();    // Hakkımda (Uzun)

        // --- İletişim (Genel) ---
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? MapFrameUrl { get; set; } // Google Maps Embed linki

        // --- Sosyal Medya ---
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? LinkedIn { get; set; }
        public string? Github { get; set; }
        public string? Instagram { get; set; }
        public string? Medium { get; set; }
    }
}