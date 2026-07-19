namespace Server.Models
{
    public class SkillEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public int OrderIndex { get; set; } = 0;
    }
}
