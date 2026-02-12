using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Portfolio.WebUI.Controllers
{
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Dili Cookie'ye kaydet (Böylece tarayıcı kapatılsa bile hatırlar)
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Geldiği sayfaya geri gönder
            return LocalRedirect(returnUrl);
        }
    }
}
