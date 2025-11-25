using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using ExpenseTrackerWebApp.Features.Budgets.Queries;
using ExpenseTrackerWebApp.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Budgets.Handlers
{
    public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, List<Budget>>
    {
        private readonly AppDbContext _context;
        public GetBudgetsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Budget>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Budgets.Where(b => b.IdentityUserId == request.UserId)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .ToListAsync();
        }
    }
}