namespace Server.CQRS.Articles
{
    public class ArticleDto
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Date { get; set; } = "";
        public string ReadTime { get; set; } = "";
        public string SubTag { get; set; } = "";
        public string Excerpt { get; set; } = "";
        public string? ImageUrl { get; set; }
        public string DetailText { get; set; } = "";
    }
}
