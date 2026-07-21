using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.CQRS.Profile;
using Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/profile/raw
        [HttpGet("raw")]
        public async Task<IActionResult> GetRawProfile()
        {
            try
            {
                var profile = await _mediator.Send(new GetRawProfileQuery());
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/profile?lang=tr
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProfile([FromQuery] string lang = "tr")
        {
            try
            {
                var profile = await _mediator.Send(new GetProfileQuery(lang));
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileEntity updatedProfile)
        {
            try
            {
                var profile = await _mediator.Send(new UpdateProfileCommand(updatedProfile));
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
