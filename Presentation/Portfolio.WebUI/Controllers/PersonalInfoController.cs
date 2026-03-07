using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.DTOs.PersonalInfo;

namespace Portfolio.WebUI.Controllers
{
    public class PersonalInfoController : Controller
    {
                IPersonalInfoService _personalInfoService;

        public PersonalInfoController(IPersonalInfoService personalInfoService)
        {
            _personalInfoService = personalInfoService;
        }
        

    }
}
