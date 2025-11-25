using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace ExpenseTrackerWebApp.Features.Budgets.Dtos
{
    public class BudgetWithProgressDto
    {
        public int Id{get; set;}
        public string? Name { get; set; }
        public BudgetType? BudgetType { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public string? IdentityUserId { get; set; }
        public decimal Spent { get; set; }
        public ICollection<BudgetCategory> BudgetCategories { get; set; } = new List<BudgetCategory>();
        public ICollection<BudgetAccount> BudgetAccounts { get; set; } = new List<BudgetAccount>();
        public int ProgressPercentage { get; set; }
        public Color ProgressColor
        {
            get
            {
                if (Progress < 75)
                {
                    return Color.Success;
                }
                else if (Progress >= 75 && Progress < 100)
                {
                    return Color.Warning;
                }
                else
                {
                    return Color.Error;
                }
            }
        }

        public int Progress
        {
            get
            {
                if (Amount == null || Amount == 0)
                    return 0;

                var progress = Math.Floor(Spent / (decimal)Amount * 100);
                return (int)progress;
            }
        }
    }
}