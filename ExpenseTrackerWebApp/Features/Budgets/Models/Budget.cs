using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Budgets.Models
{
    public class Budget{
        public int Id{get; set;}
        public string Name{get; set;}
        public BudgetType BudgetType{get; set;}

        public decimal Amount{get; set;}
        public string? Description{get; set;}
        public string IdentityUserId{get; set;}
        public IdentityUser IdentityUser { get; set; }
        public ICollection<BudgetCategory> BudgetCategories { get; set; }
        public ICollection<BudgetAccount> BudgetAccounts { get; set; }

        public Budget()
        {
            
        }
    }

    public class BudgetCategory
    {
        public int BudgetId{get; set;}
        public Budget Budget{get; set;}

        public int CategoryId{get; set;}
        public Category Category{get; set;}
    }

    public class BudgetAccount
    {
        public int BudgetId{get; set;}
        public Budget Budget{get; set;}
        public int AccountId{get; set;}
        public Account Account{get; set;}
    }


    public enum BudgetType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}