namespace Server.CQRS.Profile
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? AvatarUrl { get; set; }
        public int Repos { get; set; }
        public int Pubs { get; set; }
        public string? Github { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string? Medium { get; set; }
        public string? Tag { get; set; }
        public string? Bio { get; set; }
        public string? CvText { get; set; }
        public string? CvPdfUrl { get; set; }
        public string? CvPdfUrl_TR { get; set; }
        public string? CvPdfUrl_EN { get; set; }
        public string? CvPdfUrl_DE { get; set; }
        public string? Job { get; set; }
        public string? Education { get; set; }
        public string? Motto { get; set; }
        public bool IsOpenToOffers { get; set; }
    }
}
