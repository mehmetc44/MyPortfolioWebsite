using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Portfolio.WebUI.ViewComponents.Hero;

    public class HeroList : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
