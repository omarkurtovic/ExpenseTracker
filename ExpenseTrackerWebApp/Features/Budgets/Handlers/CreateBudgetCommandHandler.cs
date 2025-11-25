using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;

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
            var budget = new Budget()
            {
                Name = request.BudgetDto.Name!,
                BudgetType = (BudgetType)request.BudgetDto.BudgetType!,
                Amount = (decimal)request.BudgetDto.Amount!,
                IdentityUserId = request.BudgetDto.IdentityUserId!,
                Description = request.BudgetDto.Description
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            var budgetCategories = new List<BudgetCategory>();
            foreach(var category in request.BudgetDto.Categories)
            {
                var bc = new BudgetCategory()
                {
                    BudgetId = budget.Id,
                    CategoryId = category.Id
                };
                budgetCategories.Add(bc);
            }
            var budgetAccounts = new List<BudgetAccount>();
            foreach(var account in request.BudgetDto.Accounts)
            {
                var ba = new BudgetAccount()
                {
                    BudgetId = budget.Id,
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