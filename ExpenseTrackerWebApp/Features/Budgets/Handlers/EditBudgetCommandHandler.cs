using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Models;
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
            var budget = new Budget()
            {
                Name = request.BudgetDto.Name!,
                BudgetType = (BudgetType)request.BudgetDto.BudgetType!,
                Amount = (decimal)request.BudgetDto.Amount!,
                IdentityUserId = request.BudgetDto.IdentityUserId!,
                Description = request.BudgetDto.Description
            };

            var oldBudget = await _context.Budgets.SingleAsync(b => b.Id == request.Id);
            if (oldBudget != null)
            {
                oldBudget.Name = budget.Name;
                oldBudget.Amount = budget.Amount;
                oldBudget.BudgetType = budget.BudgetType;
                oldBudget.Description = budget.Description;
            }
            
            await _context.BudgetCategories.Where(bc => bc.BudgetId == request.Id).ExecuteDeleteAsync();
            await _context.BudgetAccounts.Where(ba => ba.BudgetId == request.Id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
            return budget.Id;
        }
    }

}