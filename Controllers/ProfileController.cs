using HAS.Profile.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HAS.Profile.Feature.Profile.AddAppProfile;
using static HAS.Profile.Feature.Profile.AddSubscriptionToProfile;
using static HAS.Profile.Feature.Profile.DeleteSubscriptionFromProfile;
using static HAS.Profile.Feature.Profile.GetAllAppProfilesByAccountType;
using static HAS.Profile.Feature.Profile.GetAppProfileByAuthUserId;
using static HAS.Profile.Feature.Profile.GetAppProfileByProfileId;
using static HAS.Profile.Feature.Profile.GetSubscriptionsByProfileId;
using static HAS.Profile.Feature.Profile.UpdateAppProfileToInstructor;
using static HAS.Profile.Feature.Profile.UpdateAppProfileToStudent;
using static HAS.Profile.Feature.Profile.UpdateLastLoginTimestamp;
using static HAS.Profile.Feature.Profile.UpdateLocationDetails;
using static HAS.Profile.Feature.Profile.UpdatePersonalDetails;

namespace HAS.Profile.Controllers
{
    [Authorize]
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
            var profileId = await _mediator.Send(details);

            if(string.IsNullOrEmpty(profileId))
            {
                return NotFound();
            }
            
            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
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
            var profileId = await _mediator.Send(new GetAppProfileByAuthUserIdQuery(userId));

            if(string.IsNullOrEmpty(profileId))
            {
                return NotFound();
            }

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
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

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);

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

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
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

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
        }

        [HttpPut("{profileId}/sub/rm/{instructorId}", Name = "Remove Subscription from Profile")]
        public async Task<IActionResult> RemoveSubscriptonFromProfile(string profileId, string instructorId)
        {
            var result = await _mediator.Send(new DeleteSubscriptionFromProfileCommand(profileId, instructorId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
        }

        [HttpGet("{profileId}/subs", Name = "Get Subscriptions by Profile Id")]
        public async Task<IActionResult> GetSubscriptonsByProfileId(string profileId)
        {
            var result = await _mediator.Send(new GetSubscriptionsByProfileIdQuery(profileId));

            if (result.Count() <= 0)
            {
                return Ok(new List<string>());
            }

            return Ok(result);
        }

        [HttpPut("{profileId}/ll", Name = "Update Last Login Timestamp")]
        public async Task<IActionResult> UpdateLastLoginTimestamp(string profileId)
        {
            var result = await _mediator.Send(new UpdateLastLoginTimestampCommand(profileId));

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{profileId}/upd")]
        public async Task<IActionResult> UpdatePersonalDetails(string profileId, [FromBody] UpdatePersonalDetailsCommand dto)
        {
            dto.ProfileId = profileId;

            var result = await _mediator.Send(dto);

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
        }

        [HttpPut("{profileId}/loc")]
        public async Task<IActionResult> UpdateLocation(string profileId, [FromBody] UpdateLocationDetailsCommand dto)
        {
            dto.ProfileId = profileId;

            var result = await _mediator.Send(dto);

            if (string.IsNullOrEmpty(result))
            {
                return NotFound();
            }

            var profile = await _mediator.Send(new GetAppProfileByProfileIdQuery(profileId));
            return Ok(profile);
        }
    }
}