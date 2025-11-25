using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Budgets.Dtos
{
    public class BudgetDto{
        public string? Name{get; set;}
        public BudgetType? BudgetType{get; set;}
        public decimal? Amount{get; set;}
        public string? Description{get; set;}
        public string? IdentityUserId{get; set;}
        public IEnumerable<Category>? Categories { get; set; } = new List<Category>();
        public IEnumerable<Account>? Accounts { get; set; } = new List<Account>();
    }
}