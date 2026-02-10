using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.Hero;

    public class HeroList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
