using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApi.Database.Models
{
    public class Budget{
        public int Id{get; set;}
        public string Name{get; set;} = string.Empty;
        public BudgetType BudgetType{get; set;}

        public decimal Amount{get; set;}
        public string? Description{get; set;}
        public string IdentityUserId{get; set;} = string.Empty;
        public IdentityUser IdentityUser { get; set; } = null!;
        public ICollection<BudgetCategory> BudgetCategories { get; set; } = [];
        public ICollection<BudgetAccount> BudgetAccounts { get; set; } = [];
    }

    public class BudgetCategory
    {
        public int BudgetId{get; set;}
        public Budget Budget{get; set;} = null!;

        public int CategoryId{get; set;}
        public Category Category{get; set;} = null!;
    }

    public class BudgetAccount
    {
        public int BudgetId{get; set;}
        public Budget Budget{get; set;} = null!;
        public int AccountId{get; set;}
        public Account Account{get; set;} = null!;
    }


    public enum BudgetType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}