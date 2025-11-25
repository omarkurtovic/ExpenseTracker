using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using ExpenseTrackerWebApp.Features.Budgets.Queries;
using ExpenseTrackerWebApp.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Budgets.Handlers
{
    public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, Budget?>
    {
        private readonly AppDbContext _context;
        public GetBudgetQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Budget> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            Budget? budget = await _context.Budgets
            .Where(b => b.Id == request.Id)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .FirstOrDefaultAsync();

            if(budget == null)
            {
                throw new ArgumentException("Budget not found!");
            }

            if(budget.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Budget does not belong to user!");
            }

            return budget;
        }
    }
}