using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.FAQ;

    public class FAQList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
