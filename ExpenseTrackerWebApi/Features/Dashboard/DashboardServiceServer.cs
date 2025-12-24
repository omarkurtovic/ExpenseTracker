using ExpenseTrackerSharedCL.Features.Dashboard;
using ExpenseTrackerSharedCL.Features.Dashboard.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerWebApi.Features.Dashboard.Queries;
using MediatR;
using System.Security.Claims;

namespace ExpenseTrackerWebApi.Features.Dashboard
{
    public class DashboardServiceServer : IDashboardService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(int monthsBehindToConsider = 5, int maxCategoriesToShow = 6)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var dashboardSummary = await _mediator.Send(new GetDashboardSummaryQuery()
                {
                    UserId = userId!,
                    MonthsBehindToConsider = monthsBehindToConsider,
                    MaxCategoriesToShow = maxCategoriesToShow
                });
                return Result<DashboardSummaryDto>.Success(dashboardSummary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dashboard summary: {ex.Message}");
                return Result<DashboardSummaryDto>.Failure("An error occurred while fetching dashboard stats.");
            }

            throw new NotImplementedException();
        }
    }
}
