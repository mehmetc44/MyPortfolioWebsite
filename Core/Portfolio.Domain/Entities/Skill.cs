using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Skill : BaseEntity
    {
        public MultiLanguageString Title { get; set; } = new();
        public int Value { get; set; }
    }
}
