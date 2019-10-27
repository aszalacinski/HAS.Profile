using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HAS.Profile.Feature.Profile.AddAppProfile;
using static HAS.Profile.Feature.Profile.GetProfileById;

namespace HAS.Profile.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Add Profile
        [HttpPost(Name = "Add New App Profile")]
        public async Task<IActionResult> AddNewAppProfile([FromBody] AddAppProfileCommand details)
        {
            var result = await _mediator.Send(details);

            if(string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/profile/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        // Get Profile by Id
        [HttpGet("{profileId}", Name = "Get App Profile by Profile Id")]
        public async Task<IActionResult> GetAppProfileById(string profileId)
        {
            var result = await _mediator.Send(new GetProfileByIdQuery(profileId));

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}