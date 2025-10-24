using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MyPortfolio.Models.About
{
    public class AboutModel
    {
        public string Image { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Skill> Skills { get; set; } = new List<Skill>();
    }
}