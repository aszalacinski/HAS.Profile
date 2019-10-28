using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HAS.Profile.Feature.Tribe.AddTribe;
using static HAS.Profile.Feature.Tribe.GetTribeByTribeId;

namespace HAS.Profile.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class TribeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TribeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{tribeId}", Name="Get Tribe by Tribe Id")]
        public async Task<IActionResult> GetTribeById(string tribeId)
        {
            var result = await _mediator.Send(new GetTribeByTribeIdQuery(tribeId));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("{instructorId}/a/stu", Name = "Add Student Tribe")]
        public async Task<IActionResult> AddStudentTribe(string instructorId, [FromBody] AddTribeCommand details)
        {
            details.InstructorId = instructorId;

            var result = await _mediator.Send(details);

            if(string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tribe/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);

        }
    }
}