using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPortfolio.Models.About;
using MyPortfolio.Models.Portfolio;
using MyPortfolio.Models.Resume;
namespace MyPortfolio.Models
{
    public class PageViewModel
    {
        public AboutModel About { get; set; }
        public List<ProjectCard> Projects { get; set; }
        public ResumeModel Resume { get; set; }

        public PageViewModel(AboutModel about, List<ProjectCard> projects, ResumeModel resume)
        {
            About = about;
            Projects = projects;
            Resume = resume;
        }
        
    }
}