using System.ComponentModel.DataAnnotations;

namespace Portfolio.Domain.Entities
{
    public class Language : BaseEntity
    {
        // Dilin Adı (Evrensel Format)
        // Örn: "English", "Deutsch", "Türkçe"
        [Required]
        public string Name { get; set; } = string.Empty;

        // Seviye Kodu / Açıklaması (Evrensel Format)
        // Örn: "C2 - Advanced", "Native", "B1"
        [Required]
        public string Level { get; set; } = string.Empty;

        // Görsel Bar İçin Yüzde (0-100)
        [Range(0, 100)]
        public int Percent { get; set; }
    }
}