using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Category : BaseEntity
    {
        // Kategori Adı (Örn: Web Tasarım / Web Design)
        public MultiLanguageString Name { get; set; } = new();

        // SEO için URL'de görünecek isim (Opsiyonel ama öneririm: web-tasarim)
        public string Slug { get; set; } = string.Empty;

        // İlişki: Bir kategorinin birden çok projesi olabilir
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}