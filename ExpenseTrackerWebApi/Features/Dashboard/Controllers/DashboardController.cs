using ExpenseTrackerWebApi.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Dashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] int monthsBehindToConsider = 5, [FromQuery] int maxCategoriesToShow = 6)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var dashboardSummary = await _mediator.Send(new GetDashboardSummaryQuery()
            {
                UserId = userId!,
                MonthsBehindToConsider = monthsBehindToConsider,
                MaxCategoriesToShow = maxCategoriesToShow
            });
            return Ok(dashboardSummary);
        }
    }
}