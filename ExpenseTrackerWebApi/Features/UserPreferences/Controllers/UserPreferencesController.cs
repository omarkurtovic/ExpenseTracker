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
    public class UserPreferencesController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly UserManager<IdentityUser> _userManager;

        public UserPreferencesController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPreferences()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var userPreferences = await _mediator.Send(new GetUserPreferencesQuery() { UserId = userId });
                if (userPreferences == null)
                {
                    return NotFound();
                }
                return Ok(userPreferences);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user preferences: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Route("default")]
        public async Task<IActionResult> CreateDefaultUserPreferences()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var command = new AddDefaultUserPreferencesCommand
                {
                    UserId = userId,
                };
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user preferences: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}