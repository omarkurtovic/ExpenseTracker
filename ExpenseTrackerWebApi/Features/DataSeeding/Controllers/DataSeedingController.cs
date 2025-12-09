using ExpenseTrackerSharedCL.Features.DataSeeding.Dtos;
using ExpenseTrackerWebApi.Features.Dashboard.Queries;
using ExpenseTrackerWebApi.Features.DataSeeding.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Dashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DataSeedingController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly UserManager<IdentityUser> _userManager;

        public DataSeedingController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        
        [HttpPost]
        public async Task<IActionResult> SeedUserData([FromBody] DataSeedOptionsDto dataSeedOptionsDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var command = new SeedUserCommand
            {
                UserId = userId!,
                Options = dataSeedOptionsDto
            };

            try
            {
                await _mediator.Send(command);
                return Ok("User data seeded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding user data: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}