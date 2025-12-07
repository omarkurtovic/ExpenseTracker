using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Budgets.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Budgets.Handlers
{
    public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, BudgetDto>
    {
        private readonly AppDbContext _context;
        public GetBudgetQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<BudgetDto> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            var budget = await _context.Budgets
            .Where(b => b.Id == request.Id)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .FirstOrDefaultAsync(cancellationToken);

            if(budget == null)
            {
                throw new ArgumentException("Budget not found!");
            }

            if(budget.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Budget does not belong to user!");
            }

            return new BudgetDto()
            {
                Name = budget.Name,
                BudgetType = (BudgetType?)budget.BudgetType,
                Amount = budget.Amount,
                Description = budget.Description,
                Categories = budget.BudgetCategories.Select(bc => bc.CategoryId).ToList(),
                Accounts = budget.BudgetAccounts.Select(ba => ba.AccountId).ToList()
            };
        }
    }
}