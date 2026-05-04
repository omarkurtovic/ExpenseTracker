using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using ExpenseTrackerWebApi.Features.Features.UserPreferences.Queries;
using ExpenseTrackerWebApi.Features.UserPreferences.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.UserPreferences.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserPreferencesController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> GetUserPreferences()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var userPreferences = await _mediator.Send(new GetUserPreferencesQuery() { UserId = userId });
            if (userPreferences == null)
            {
                return NotFound();
            }
            return Ok(userPreferences);
        }

        [HttpPost]
        [Route("default")]
        public async Task<IActionResult> CreateDefaultUserPreferences()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var command = new AddDefaultUserPreferencesCommand
            {
                UserId = userId,
            };
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditUserPreferences([FromBody] UserPreferenceDto userPreferencesDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var command = new EditUserPreferencesCommand
            {
                UserId = userId,
                UserPreferencesDto = userPreferencesDto
            };
            await _mediator.Send(command);
            return Ok();
        }
    }
}