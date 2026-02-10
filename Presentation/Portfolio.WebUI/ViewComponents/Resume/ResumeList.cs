using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.Resume;

    public class ResumeList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
