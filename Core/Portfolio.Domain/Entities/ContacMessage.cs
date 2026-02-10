using System.ComponentModel.DataAnnotations;

namespace Portfolio.Domain.Entities
{
    public class ContactMessage : BaseEntity
    {
        [Required]
        public string SenderName { get; set; } = string.Empty; // Gönderen Adı

        [Required]
        [EmailAddress] // Email formatı kontrolü için
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty; // Konu

        [Required]
        public string Message { get; set; } = string.Empty; // Mesaj İçeriği

        // Admin panelde "Koyu Renk / Okunmamış" göstermek için
        public bool IsRead { get; set; } = false; 
    }
}