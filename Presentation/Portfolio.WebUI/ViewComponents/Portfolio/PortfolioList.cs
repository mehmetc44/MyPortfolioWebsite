using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.Portfolio;

    public class PortfolioList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
