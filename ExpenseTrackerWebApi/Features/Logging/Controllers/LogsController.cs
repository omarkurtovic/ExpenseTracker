using ExpenseTrackerSharedCL.Features.Logging.Dtos;
using ExpenseTrackerWebApi.Features.Logging.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Logging.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly UserManager<IdentityUser> _userManager;

        public LogsController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("logs")]
        public async Task<IActionResult> GetLogs([FromBody] LogsGridOptionsDto logOptions)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var logs = await _mediator.Send(new GetLogsPageDataQuery() { UserId = userId, LogOptions = logOptions });
                return Ok(logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting logs: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}