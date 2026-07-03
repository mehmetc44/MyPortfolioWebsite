namespace Server.Models
{
    public class ProjectEntity
    {
        public string Id { get; set; } = ""; // Unique URL identifier key
        public string Title_TR { get; set; } = "";
        public string Title_EN { get; set; } = "";
        public string Title_DE { get; set; } = "";
        public string Category { get; set; } = "";
        public string Date { get; set; } = "";
        public string Client { get; set; } = "";
        public string SubTag_TR { get; set; } = "";
        public string SubTag_EN { get; set; } = "";
        public string SubTag_DE { get; set; } = "";
        public string Description_TR { get; set; } = "";
        public string Description_EN { get; set; } = "";
        public string Description_DE { get; set; } = "";
        public string Tech { get; set; } = "";
        public string RepoUrl { get; set; } = "";
        public string DemoUrl { get; set; } = "";
        public string ImagesJson { get; set; } = "[]"; // Serialized images list
        public string DetailText_TR { get; set; } = "";
        public string DetailText_EN { get; set; } = "";
        public string DetailText_DE { get; set; } = "";
    }
}
