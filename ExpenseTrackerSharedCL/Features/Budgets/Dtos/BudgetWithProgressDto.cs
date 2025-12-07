using MudBlazor;

namespace ExpenseTrackerSharedCL.Features.Budgets.Dtos
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
        public ICollection<BudgetCategoryDto> BudgetCategories { get; set; } = new List<BudgetCategoryDto>();
        public ICollection<BudgetAccountDto> BudgetAccounts { get; set; } = new List<BudgetAccountDto>();
        public int ProgressPercentage { get; set; }
        public Color ProgressColor
        {
            get
            {
                if (Progress < 50)
                {
                    return Color.Success;
                }
                else if (Progress >= 50 && Progress < 75)
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
                if(progress > 100)
                    progress = 100;
                return (int)progress;
            }
        }
    }
}