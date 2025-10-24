using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models.Portfolio
{
    public class ProjectCard
    {
        public int Id { get; set; }                     
        public string Title { get; set; } = string.Empty; 
        public string ShortDescription { get; set; } = string.Empty; 
        public string ThumbnailImage { get; set; } = string.Empty;                          
        public string Category { get; set; } = string.Empty;                         
    }
}