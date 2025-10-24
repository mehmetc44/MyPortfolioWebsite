using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models.About
{
    public class Skill
    {
        public string SkillName { get; set; } = string.Empty;
        public int Proficiency { get; set; } = 0; // Proficiency as a percentage (0-100)
    }
}