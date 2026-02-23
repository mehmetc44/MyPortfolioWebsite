using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.WebUI.Models.About;
using Portfolio.WebUI.Models.Portfolio;
using Portfolio.WebUI.Models.Resume;
using Portfolio.WebUI.Models.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Localization; 
using Microsoft.AspNetCore.Http; 
namespace Portfolio.WebUI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
    }
}