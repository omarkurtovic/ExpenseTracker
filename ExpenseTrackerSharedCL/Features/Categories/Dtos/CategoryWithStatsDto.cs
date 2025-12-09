
namespace ExpenseTrackerSharedCL.Features.Categories.Dtos
{
    public class CategoryWithStatsDto
    {
        public int? Id{get; set;}
        public string? Name{get; set;}
        public TransactionTypeDto? Type { get; set; }
        public string? Icon{get; set;}
        public string? Color{get; set;}
        public int TransactionsCount{get; set;}
        public decimal TotalAmount{get; set;}
        public decimal CurrentMonthAmount{get; set;}
        public decimal LastMonthAmount{get; set;}

        public MudBlazor.Color ComparisonColor
        {
            get
            {
                if (LastMonthAmount == 0)
                return MudBlazor.Color.Default;

                if(Type == TransactionTypeDto.Expense)
                {
                    if (CurrentMonthAmount < LastMonthAmount)
                        return MudBlazor.Color.Success;
                    else if (CurrentMonthAmount > LastMonthAmount)
                        return MudBlazor.Color.Error;    
                    else
                        return MudBlazor.Color.Default;
                }
                else if(Type == TransactionTypeDto.Income)
                {
                    if (CurrentMonthAmount > LastMonthAmount)
                        return MudBlazor.Color.Success;
                    else if (CurrentMonthAmount < LastMonthAmount)
                        return MudBlazor.Color.Error;    
                    else
                        return MudBlazor.Color.Default;
                }
                else
                {
                    return MudBlazor.Color.Default;
                }

            }
        }

        public string ComparisonDisplay
        {
            get
            {
                if (LastMonthAmount == 0)
                    return "—";

                var percentChange = LastMonthAmount == 0 ? 0 : Math.Abs(((CurrentMonthAmount - LastMonthAmount) / LastMonthAmount) * 100);
                var indicator = CurrentMonthAmount > LastMonthAmount ? "↑" : (CurrentMonthAmount < LastMonthAmount ? "↓" : "—");

                return $"{indicator} {Math.Round(percentChange, 1)}%";
            }
        }
            
    }
}