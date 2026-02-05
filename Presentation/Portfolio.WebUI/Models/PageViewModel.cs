using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Portfolio.WebUI.Models.About;
using Portfolio.WebUI.Models.Portfolio;
using Portfolio.WebUI.Models.Resume;
namespace Portfolio.WebUI.Models.Models
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