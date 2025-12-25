using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerSharedCL.Features.Budgets.Services;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerWebApi.Features.Budgets.Queries;
using MediatR;
using System.Security.Claims;

namespace ExpenseTrackerWebApi.Features.Budgets.Services
{
    public class BudgetServiceServer : IBudgetService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BudgetServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<Result> CreateBudgetAsync(BudgetDto budgetDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteBudgetAsync(int budgetId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> EditBudgetAsync(BudgetDto budgetDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<BudgetWithProgressDto>>> GetBudgetsAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var budgets = await _mediator.Send(new GetBudgetsWithProgressQuery() { UserId = userId });
                return Result<List<BudgetWithProgressDto>>.Success(budgets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting budgets with progress: {ex.Message}");
                return Result<List<BudgetWithProgressDto>>.Failure("An error occurred while fetching budgets.");
            }
        }
    }
}
