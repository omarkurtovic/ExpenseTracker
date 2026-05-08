using ExpenseTrackerSharedCL.Features.DataSeeding.Dtos;
using ExpenseTrackerWebApi.Features.Dashboard.Queries;
using ExpenseTrackerWebApi.Features.DataSeeding.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.DataSeeding.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DataSeedingController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost]
        public async Task<IActionResult> SeedUserData([FromBody] DataSeedOptionsDto dataSeedOptionsDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var command = new SeedUserCommand
            {
                UserId = userId!,
                Options = dataSeedOptionsDto
            };

            await _mediator.Send(command);
            return Ok("User data seeded successfully.");
        }
    }
}