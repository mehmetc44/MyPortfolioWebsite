using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class AboutMe : BaseEntity
    {
        public string? HeroImagePath {get; set; }
        public string? ProfileImagePath { get; set; }
        public MultiLanguageString Biography { get; set; } = new();   
        public string? Email { get; set; }
        public MultiLanguageString Title { get; set; } = new();
        public string? Address { get; set; }
        public string? Telegram { get; set; }
        public string? LinkedIn { get; set; }
        public string? Github { get; set; }
        public string? Instagram { get; set; }
        public string? Medium {get; set;}
    }
}