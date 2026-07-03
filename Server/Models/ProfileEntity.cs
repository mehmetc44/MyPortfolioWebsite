namespace Server.Models
{
    public class ProfileEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Tag_TR { get; set; } = "";
        public string Tag_EN { get; set; } = "";
        public string Tag_DE { get; set; } = "";
        public string Title_TR { get; set; } = "";
        public string Title_EN { get; set; } = "";
        public string Title_DE { get; set; } = "";
        public string Bio_TR { get; set; } = "";
        public string Bio_EN { get; set; } = "";
        public string Bio_DE { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public int Repos { get; set; }
        public int Pubs { get; set; }
        public string Github { get; set; } = "";
        public string Linkedin { get; set; } = "";
        public string Instagram { get; set; } = "";
        public string Medium { get; set; } = "";
    }
}
