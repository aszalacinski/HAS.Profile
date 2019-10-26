using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // Get Profile by Id
        [HttpGet("{profileId}", Name = "Get Profile by Profile Id")]
        public async Task<IActionResult> GetProfileById(string profileId)
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