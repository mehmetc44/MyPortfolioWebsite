using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models.Resume
{
    public class ResumeModel
    {
        public string Summary { get; set; } = string.Empty;
        public List<Education> EducationList { get; set; } = new List<Education>();
        public List<Experience> ExperienceList { get; set; } = new List<Experience>();
    }
}