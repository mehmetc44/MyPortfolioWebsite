namespace Portfolio.Domain.Entities
{
    public class ProjectImage : BaseEntity
    {
        public string Url { get; set; } = null!;
        
        // Hangi projeye ait?
        public Guid ProjectId { get; set; }
        
        // Navigation Property (İlişki)
        public Project Project { get; set; } = null!;
        
        // Galerideki sıralaması için (Opsiyonel ama öneririm)
        public int Order { get; set; } 
    }
}