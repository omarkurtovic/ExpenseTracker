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
        private readonly ICurrentUserService _currentUserService;
        public GetBudgetQueryHandler(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<Budget?> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            return await _context.Budgets.Where(b => b.Id == request.Id)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .FirstOrDefaultAsync();
        }
    }
}