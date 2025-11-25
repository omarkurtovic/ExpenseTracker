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
        private readonly ICurrentUserService _currentUserService;
        public GetBudgetsQueryHandler(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<List<Budget>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            return await _context.Budgets.Where(b => b.IdentityUserId == userId)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .ToListAsync();
        }
    }
}