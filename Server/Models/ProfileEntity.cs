namespace Server.Models
{
    public class ProfileEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Tag_TR { get; set; }
        public string? Tag_EN { get; set; }
        public string? Tag_DE { get; set; }
        public string? Bio_TR { get; set; }
        public string? Bio_EN { get; set; }
        public string? Bio_DE { get; set; }
        public string? AvatarUrl { get; set; }
        public int Repos { get; set; }
        public int Pubs { get; set; }
        public string? Github { get; set; }
        public string? Linkedin { get; set; }
        public string? Instagram { get; set; }
        public string? Medium { get; set; }
        public string? CvText_TR { get; set; }
        public string? CvText_EN { get; set; }
        public string? CvText_DE { get; set; }
        public string? CvPdfUrl_TR { get; set; }
        public string? CvPdfUrl_EN { get; set; }
        public string? CvPdfUrl_DE { get; set; }

        // New configurable profile details
        public string? Job_TR { get; set; }
        public string? Job_EN { get; set; }
        public string? Job_DE { get; set; }

        public string? Education_TR { get; set; }
        public string? Education_EN { get; set; }
        public string? Education_DE { get; set; }

        public string? Motto_TR { get; set; }
        public string? Motto_EN { get; set; }
        public string? Motto_DE { get; set; }

        public bool IsOpenToOffers { get; set; }
    }
}
