namespace Server.Models
{
    public class ArticleEntity
    {
        public string Id { get; set; } = ""; // Unique URL key
        public string Title_TR { get; set; } = "";
        public string Title_EN { get; set; } = "";
        public string Title_DE { get; set; } = "";
        public string CategoryId { get; set; } = ""; // Points to the SubCategory Id
        public string Date { get; set; } = "";
        public string ReadTime { get; set; } = "";
        public string Excerpt_TR { get; set; } = "";
        public string Excerpt_EN { get; set; } = "";
        public string Excerpt_DE { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string DetailText_TR { get; set; } = "";
        public string DetailText_EN { get; set; } = "";
        public string DetailText_DE { get; set; } = "";
        public int OrderIndex { get; set; } = 0;
        public bool IsDraft { get; set; } = false;
    }
}
