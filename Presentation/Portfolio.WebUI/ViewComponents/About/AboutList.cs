using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.About;

    public class AboutList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
