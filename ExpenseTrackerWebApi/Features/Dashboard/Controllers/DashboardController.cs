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
    public class DashboardController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] int monthsBehindToConsider = 5, [FromQuery] int maxCategoriesToShow = 6)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dashboard summary: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}