using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.Services;

    public class ServicesList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
