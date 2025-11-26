using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace ExpenseTrackerWebApp.Features.Budgets.Handlers
{
    public class EditBudgetCommandHandler : IRequestHandler<EditBudgetCommand, int>
    {
        
        private readonly AppDbContext _context;

        public EditBudgetCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditBudgetCommand request, CancellationToken cancellationToken)
        {
            var oldBudget = await _context.Budgets
                .Where(b => b.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken); 

            if(oldBudget == null)
            {
                throw new ArgumentException("Budget not found!");
            }

            if(oldBudget.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Budget does not belong to user!");
            }

            var userCategoryIds = await _context.Categories
                .Where(c => c.IdentityUserId == request.UserId)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var userAccountIds = await _context.Accounts
                .Where(a => a.IdentityUserId == request.UserId)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            if (!request.BudgetDto.Categories!.Select(c => c.Id).All(id => userCategoryIds.Contains(id)))
                throw new UnauthorizedAccessException("Category does not belong to user");

            if (!request.BudgetDto.Accounts!.Select(c => c.Id).All(id => userAccountIds.Contains(id)))
                throw new UnauthorizedAccessException("Account does not belong to user");


            var budget = new Budget()
            {
                Name = request.BudgetDto.Name!,
                BudgetType = (BudgetType)request.BudgetDto.BudgetType!,
                Amount = (decimal)request.BudgetDto.Amount!,
                IdentityUserId = request.UserId,
                Description = request.BudgetDto.Description
            };

            oldBudget.Name = budget.Name;
            oldBudget.Amount = budget.Amount;
            oldBudget.BudgetType = budget.BudgetType;
            oldBudget.Description = budget.Description;
            
            await _context.BudgetCategories.Where(bc => bc.BudgetId == request.Id).ExecuteDeleteAsync(cancellationToken);
            await _context.BudgetAccounts.Where(ba => ba.BudgetId == request.Id).ExecuteDeleteAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _context.ChangeTracker.Clear();

            var budgetCategories = new List<BudgetCategory>();
            foreach(var category in request.BudgetDto.Categories)
            {
                var bc = new BudgetCategory()
                {
                    BudgetId = request.Id,
                    CategoryId = category.Id
                };
                budgetCategories.Add(bc);
            }
            var budgetAccounts = new List<BudgetAccount>();
            foreach(var account in request.BudgetDto.Accounts)
            {
                var ba = new BudgetAccount()
                {
                    BudgetId = request.Id,
                    AccountId = account.Id
                };
                budgetAccounts.Add(ba);
            }

            _context.BudgetCategories.AddRange(budgetCategories);
            _context.BudgetAccounts.AddRange(budgetAccounts);
            await _context.SaveChangesAsync(cancellationToken);
            return budget.Id;
        }
    }

}