using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebUI.Controllers
{
    public class AuthController : Controller
    {
        // GET: AuthController
        public ActionResult LogIn()
        {
            return View();
        }

    }
}
