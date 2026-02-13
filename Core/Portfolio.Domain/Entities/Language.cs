using System.ComponentModel.DataAnnotations;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities
{
    public class Language : BaseEntity
    {
        [Required]
        public MultiLanguageString Name { get; set; } = null!;
        [Required]
        public string Level { get; set; } = string.Empty;
    }
}