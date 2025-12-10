namespace ExpenseTrackerSharedCL.Features.Transactions.Dtos
{
    public class TransactionsGridOptionsDto
    {
        public bool IsReoccuring{get; set;} = false;
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 0;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        public TransactionsFiltersDto Filters { get; set; } = new TransactionsFiltersDto();
    }
}