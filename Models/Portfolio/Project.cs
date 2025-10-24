using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models.Portfolio
{
    public class Project
    {
        public int Id { get; set; }                     
        public string Title { get; set; } = string.Empty; 
        public string ShortDescription { get; set; } = string.Empty; 
        public string LongDescription { get; set; } = string.Empty; 
        public string ThumbnailImage { get; set; } = string.Empty;   
        public List<string> GalleryImages { get; set; } = new();     
        public string? ProjectUrl { get; set; }                      
        public string Category { get; set; }  = string.Empty;                      
        public string? Client { get; set; }    
        public string? ProjectDate { get; set; }                  
    }

}