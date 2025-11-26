using ExpenseTrackerWebApp.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Budgets.Dtos
{
    public class BudgetDto{
        public string? Name{get; set;}
        public BudgetType? BudgetType{get; set;}
        public decimal? Amount{get; set;}
        public string? Description{get; set;}
        public IEnumerable<int>? Categories { get; set; } = new List<int>();
        public IEnumerable<int>? Accounts { get; set; } = new List<int>();
    }
}