using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Budgets.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Budgets.Handlers
{
    public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, List<BudgetDto>>
    {
        private readonly AppDbContext _context;
        public GetBudgetsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        {
            var budgets = await _context.Budgets.Where(b => b.IdentityUserId == request.UserId)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .ToListAsync();

            return budgets.Select(budget => new BudgetDto()
            {
                Name = budget.Name,
                BudgetType = (ExpenseTrackerSharedCL.Features.Budgets.Dtos.BudgetType?)budget.BudgetType,
                Amount = budget.Amount,
                Description = budget.Description,
                Categories = budget.BudgetCategories.Select(bc => bc.CategoryId).ToList(),
                Accounts = budget.BudgetAccounts.Select(ba => ba.AccountId).ToList()
            }).ToList();
        }
    }
}