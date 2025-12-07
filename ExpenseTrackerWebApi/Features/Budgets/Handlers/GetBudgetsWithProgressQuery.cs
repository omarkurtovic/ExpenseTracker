using System.Globalization;
using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Budgets.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;

namespace ExpenseTrackerWebApi.Features.Budgets.Handlers
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
                BudgetType = (ExpenseTrackerSharedCL.Features.Budgets.Dtos.BudgetType?)b.BudgetType,
                Description = b.Description,
                IdentityUserId = b.IdentityUserId,
                BudgetCategories = b.BudgetCategories.Select(bc => new BudgetCategoryDto(){
                    BudgetId = bc.BudgetId,
                    CategoryId = bc.CategoryId,
                }).ToList(),
                BudgetAccounts = b.BudgetAccounts.Select(ba => new BudgetAccountDto(){
                    BudgetId = ba.BudgetId,
                    AccountId = ba.AccountId,
                }).ToList(),
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
            var categories = budget.BudgetCategories.Select(bc => bc.CategoryId).ToList().ToHashSet();
            var accounts = budget.BudgetAccounts.Select(ba => ba.AccountId).ToList().ToHashSet();

            filteredTransctions = [.. transactions.Where(t => t.IsReoccuring == null || !(bool)t.IsReoccuring)
            .Where(t => categories.ToHashSet().Contains(t.CategoryId))
            .Where(t => accounts.ToHashSet().Contains(t.AccountId))];
            switch(budget.BudgetType){
                case ExpenseTrackerSharedCL.Features.Budgets.Dtos.BudgetType.Weekly:
                    var weekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    var weekEnd = weekStart.AddDays(7).AddSeconds(-1);
                    filteredTransctions = [.. filteredTransctions.Where(t => t.Date >= weekStart && t.Date <= weekEnd)];
                    break;
                case ExpenseTrackerSharedCL.Features.Budgets.Dtos.BudgetType.Monthly:
                    var monthStart = DateTime.Now.StartOfMonth(CultureInfo.CurrentCulture);
                    var monthEnd = DateTime.Now.EndOfMonth(CultureInfo.CurrentCulture) + new TimeSpan(23, 59, 59);
                    filteredTransctions = [.. filteredTransctions.Where(t => t.Date >= monthStart && t.Date <= monthEnd)];
                    break;
                case ExpenseTrackerSharedCL.Features.Budgets.Dtos.BudgetType.Yearly:
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