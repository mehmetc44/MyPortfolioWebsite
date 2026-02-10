using System;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.ViewComponents.Contact;

    public class ContactList : ViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
