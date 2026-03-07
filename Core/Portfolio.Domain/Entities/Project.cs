using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{

    public class Project : BaseEntity
    {
        public MultiLanguageString Title { get; set; } = new();
        public MultiLanguageString Description { get; set; } = new();
        public Guid? CoverImageId { get; set; }
        public ProjectImageFile CoverImage { get; set; } = new();
        public ICollection<ProjectImageFile> Images { get; set; } = new List<ProjectImageFile>();
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string? Client { get; set; }
        public string? ProjectUrl { get; set; }
        public DateTime Date { get; set; }
        public int OrderNo { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
