using System.Globalization;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;

namespace ExpenseTrackerWebApp.Features.Budgets.Handlers
{
    public class GetBudgetsWithProgressQueryHandler : IRequestHandler<GetBudgetsWithProgressQuery, List<BudgetWithProgressDto>>
    {
        private readonly AppDbContext _context;
        public GetBudgetsWithProgressQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<BudgetWithProgressDto>> Handle(GetBudgetsWithProgressQuery request, CancellationToken cancellationToken)
        {
            var budgetDtos = await _context.Budgets.Where(b => b.IdentityUserId == request.UserId)
            .Include(b => b.BudgetCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.BudgetAccounts).ThenInclude(ba => ba.Account)
            .Select(b => new BudgetWithProgressDto()
            {
                Id = b.Id,
                Name = b.Name,
                Amount = b.Amount,
                BudgetType = b.BudgetType,
                Description = b.Description,
                IdentityUserId = b.IdentityUserId,
                BudgetCategories = b.BudgetCategories,
                BudgetAccounts = b.BudgetAccounts,
            })
            .ToListAsync();

            var allTransactions = await _context.Transactions
                .Where(t => t.Account.IdentityUserId == request.UserId)
                .Include(t => t.Account)
                .Include(t => t.Category)
                .ToListAsync();



            var result = new List<BudgetWithProgressDto>();
            foreach(var budgetDto in budgetDtos)
            {
                budgetDto.Spent = GetBudgetSpent(budgetDto, allTransactions);
                result.Add(budgetDto);
            }

            return result;
        }

        private decimal GetBudgetSpent(BudgetWithProgressDto budget, List<Transaction> transactions){

            var filteredTransctions = new List<Transaction>();
            var categories = budget.BudgetCategories.Select(bc => bc.Category).ToList().ToHashSet();
            var accounts = budget.BudgetAccounts.Select(ba => ba.Account).ToList().ToHashSet();

            filteredTransctions = [.. transactions.Where(t => t.IsReoccuring == null || !(bool)t.IsReoccuring)
            .Where(t => categories.ToHashSet().Contains(t.Category))
            .Where(t => accounts.ToHashSet().Contains(t.Account))];

            switch(budget.BudgetType){
                case BudgetType.Weekly:
                    var weekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    var weekEnd = weekStart.AddDays(7).AddSeconds(-1);
                    filteredTransctions = [.. filteredTransctions.Where(t => t.Date >= weekStart && t.Date <= weekEnd)];
                    break;
                case BudgetType.Monthly:
                    var monthStart = DateTime.Now.StartOfMonth(CultureInfo.CurrentCulture);
                    var monthEnd = DateTime.Now.EndOfMonth(CultureInfo.CurrentCulture);
                    filteredTransctions = [.. filteredTransctions.Where(t => t.Date >= monthStart && t.Date <= monthEnd)];
                    break;
                case BudgetType.Yearly:
                    var yearStart = new DateTime(DateTime.Now.Year, 1, 1);
                    var yearEnd = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
                    filteredTransctions = [.. filteredTransctions.Where(t => t.Date >= yearStart && t.Date <= yearEnd)];
                    break;
                default:
                    break;
            }

            return Math.Abs(filteredTransctions.Sum(t => t.Amount));
        }

        
    }
}