using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HAS.Profile.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HAS.Profile.Feature.Profile.AddAppProfile;
using static HAS.Profile.Feature.Profile.AddSubscriptionToProfile;
using static HAS.Profile.Feature.Profile.DeleteSubscriptionFromProfile;
using static HAS.Profile.Feature.Profile.GetAllAppProfilesByAccountType;
using static HAS.Profile.Feature.Profile.GetAppProfileByAuthUserId;
using static HAS.Profile.Feature.Profile.GetAppProfileByProfileId;
using static HAS.Profile.Feature.Profile.UpdateAppProfileToInstructor;
using static HAS.Profile.Feature.Profile.UpdateAppProfileToStudent;

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

        // Add App Profile
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

        // Get App Profile by Profile Id
        [HttpGet("{profileId}", Name = "Get App Profile by Profile Id")]
        public async Task<IActionResult> GetAppProfileById(string profileId)
        {
            var result = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // Get App Profile by Auth User Id
        [HttpGet("by/{userId}", Name ="Get App Profile by Auth User Id")]
        public async Task<IActionResult> GetAppProfileByAuthUserId(string userId)
        {
            var result = await _mediator.Send(new GetAppProfileByAuthUserIdQuery(userId));

            if(string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/profile/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        // Update Profile to Instructor
        [HttpPut("{profileId}/as/in", Name ="Update Profile to Instructor")]
        public async Task<IActionResult> UpdateProfileToInstructor(string profileId)
        {
            var result = await _mediator.Send(new UpdateAppProfileToInstructorCommand(profileId));

            if(string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/profile/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);

        }

        // Update Profile to Student
        [HttpPut("{profileId}/as/st", Name = "Update Profile to Student")]
        public async Task<IActionResult> UpdateProfileToStudent(string profileId)
        {
            var result = await _mediator.Send(new UpdateAppProfileToStudentCommand(profileId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/profile/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        [HttpGet("students", Name = "Get All Student Profiles")]
        public async Task<IActionResult> GetAllStudentProfiles()
        {
            var result = await _mediator.Send(new GetAllAppProfilesByAccountTypeQuery(ProfileConstants.STUDENT));

            if (result.Count() <= 0)
            {
                return Ok(new List<string>());
            }

            return Ok(result);
        }

        [HttpGet("instructors", Name = "Get All Instructor Profiles")]
        public async Task<IActionResult> GetAllInstructorProfiles()
        {
            var result = await _mediator.Send(new GetAllAppProfilesByAccountTypeQuery(ProfileConstants.INSTRUCTOR));

            if (result.Count() <= 0)
            {
                return Ok(new List<string>());
            }

            return Ok(result);
        }

        [HttpPut("{profileId}/sub/add/{instructorId}", Name = "Add Subscription to Profile")]
        public async Task<IActionResult> AddSubscriptionToProfile(string profileId, string instructorId)
        {
            var result = await _mediator.Send(new AddSubscriptionToProfileCommand(profileId, instructorId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/profile/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }

        [HttpPut("{profileId}/sub/rm/{instructorId}", Name = "Remove Subscription from Profile")]
        public async Task<IActionResult> RemoveSubscriptonFromProfile(string profileId, string instructorId)
        {
            var result = await _mediator.Send(new DeleteSubscriptionFromProfileCommand(profileId, instructorId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/profile/{result}";

            Response.Headers.Add("Location", uri);
            return StatusCode(303);
        }
    }
}