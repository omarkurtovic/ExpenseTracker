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
    [Authorize(Roles = "Administrator")]
    public class LogsController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> GetLogs([FromBody] LogsGridOptionsDto logOptions)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var logs = await _mediator.Send(new GetLogsPageDataQuery() { UserId = userId, LogOptions = logOptions });
            return Ok(logs);
        }
    }
}