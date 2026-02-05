using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.WebUI.Models.Resume
{
    public class Education
    {
        public string University { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}