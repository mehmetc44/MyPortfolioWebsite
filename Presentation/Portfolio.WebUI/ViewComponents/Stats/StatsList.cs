using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.Stats;

    public class StatsList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
