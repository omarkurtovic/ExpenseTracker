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
        public async Task<Budget?> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            return await _context.Budgets.Where(b => b.Id == request.Id)
            .Where(b => b.IdentityUserId == request.UserId)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .FirstOrDefaultAsync();
        }
    }
}