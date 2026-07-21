namespace Server.CQRS.Projects
{
    public class ProjectDto
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Date { get; set; } = "";
        public string? Client { get; set; }
        public string SubTag { get; set; } = "";
        public string Description { get; set; } = "";
        public string Tech { get; set; } = "";
        public string? RepoUrl { get; set; }
        public string? DemoUrl { get; set; }
        public string? ImagesJson { get; set; }
        public string DetailText { get; set; } = "";
    }
}
