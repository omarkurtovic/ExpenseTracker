using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Budgets.Handlers
{
    public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateBudgetCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var userCategoryIds = await _context.Categories
                .Where(c => c.IdentityUserId == request.UserId)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var userAccountIds = await _context.Accounts
                .Where(a => a.IdentityUserId == request.UserId)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

        if (!request.BudgetDto.Categories!.All(id => userCategoryIds.Contains(id)))
            throw new UnauthorizedAccessException("Category does not belong to user");

        if (!request.BudgetDto.Accounts!.All(id => userAccountIds.Contains(id)))
            throw new UnauthorizedAccessException("Account does not belong to user");

            var budget = new Budget()
            {
                Name = request.BudgetDto.Name!,
                BudgetType = (BudgetType)request.BudgetDto.BudgetType!,
                Amount = (decimal)request.BudgetDto.Amount!,
                IdentityUserId = request.UserId,
                Description = request.BudgetDto.Description
            };


            var budgetCategories = new List<BudgetCategory>();
            foreach(var categoryId in request.BudgetDto.Categories)
            {
                var bc = new BudgetCategory()
                {
                    BudgetId = budget.Id,
                    CategoryId = categoryId
                };
                budgetCategories.Add(bc);
            }
            var budgetAccounts = new List<BudgetAccount>();
            foreach(var accountId in request.BudgetDto.Accounts)
            {
                var ba = new BudgetAccount()
                {
                    BudgetId = budget.Id,
                    AccountId = accountId
                };
                budgetAccounts.Add(ba);
            }

            _context.Budgets.Add(budget);
            _context.BudgetCategories.AddRange(budgetCategories);
            _context.BudgetAccounts.AddRange(budgetAccounts);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return budget.Id;
        }
    }

}