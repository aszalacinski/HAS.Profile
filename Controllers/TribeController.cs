using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HAS.Profile.Feature.Tribe.AddStudentToTribe;
using static HAS.Profile.Feature.Tribe.AddStudentTribe;
using static HAS.Profile.Feature.Tribe.DeleteStudentFromTribe;
using static HAS.Profile.Feature.Tribe.DeleteTribe;
using static HAS.Profile.Feature.Tribe.GetAllTribesByStudent;
using static HAS.Profile.Feature.Tribe.GetTribeByInstructorId;
using static HAS.Profile.Feature.Tribe.GetTribeByTribeId;
using static HAS.Profile.Feature.Tribe.UpdateTribe;
using static HAS.Profile.Feature.Tribe.UpdateTribeToNonSubscription;
using static HAS.Profile.Feature.Tribe.UpdateTribeToSubscription;

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

        [HttpGet("{instructorId}/a", Name = "Get all Tribes by Instructor Id")]
        public async Task<IActionResult> GetAllTribesByInstructorId(string instructorId)
        {
            var result = await _mediator.Send(new GetTribeByInstructorIdQuery(instructorId));

            if (result.Count() <= 0)
            {
                return Ok(new List<string>());
            }

            return Ok(result);
        }

        [HttpPost("{instructorId}/a/stu", Name = "Add Student Tribe")]
        public async Task<IActionResult> AddStudentTribe(string instructorId, [FromBody] AddStudentTribeCommand details)
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

        [HttpDelete("{instructorId}/arc/{tribeId}", Name = "Delete Tribe")]
        public async Task<IActionResult> DeleteTribe(string tribeId, string instructorId)
        {
            var result = await _mediator.Send(new DeleteTribeCommand(tribeId, instructorId));

            if (result > 0)
            {
                return NoContent();
            }

            return BadRequest($"No tribes were deleted");
        }

        [HttpPut("{instructorId}/{tribeId}/sub", Name = "Set Tribe to Subscription")]
        public async Task<IActionResult> SetTribeToSubscription(string tribeId, string instructorId)
        {
            var result = await _mediator.Send(new UpdateTribeToSubscriptionCommand(tribeId, instructorId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tribe/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        [HttpPut("{instructorId}/{tribeId}/nsub", Name = "Set Tribe to Non Subscription")]
        public async Task<IActionResult> SetTribeToNonSubscription(string tribeId, string instructorId)
        {
            var result = await _mediator.Send(new UpdateTribeToNonSubscriptionCommand(tribeId, instructorId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tribe/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }


        [HttpPut("{instructorId}/{tribeId}/a/{studentId}", Name = "Add Student to a Tribe")]
        public async Task<IActionResult> AddStudentToTribe(string instructorId, string tribeId, string studentId)
        {
            var result = await _mediator.Send(new AddStudentToTribeCommand(instructorId, tribeId, studentId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tribe/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        [HttpDelete("{instructorId}/{tribeId}/r/{studentId}", Name = "Delete Student from Tribe")]
        public async Task<IActionResult> DeleteStudentFromTribe(string instructorId, string tribeId, string studentId)
        {
            var result = await _mediator.Send(new DeleteStudentFromTribeCommand(instructorId, tribeId, studentId));

            if (string.IsNullOrEmpty(result))
            {
                return NoContent();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tribe/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        [HttpGet("{studentId}/s", Name = "Get All Tribes By Student")]
        public async Task<IActionResult> GetAllTribesByStudent(string studentId)
        {
            var result = await _mediator.Send(new GetAllTribesByStudentQuery(studentId));

            if (result.Count() <= 0)
            {
                return Ok(new List<string>());
            }

            return Ok(result);
        }

        [HttpPut("{instructorId}/u/{tribeId}")]
        public async Task<IActionResult> UpdateTribe(string instructorId, string tribeId, [FromBody] UpdateTribeCommand dto)
        {
            dto.InstructorId = instructorId;
            dto.TribeId = tribeId;

            var result = await _mediator.Send(dto);

            if (string.IsNullOrEmpty(result))
            {
                return NoContent();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tribe/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }
    }
}