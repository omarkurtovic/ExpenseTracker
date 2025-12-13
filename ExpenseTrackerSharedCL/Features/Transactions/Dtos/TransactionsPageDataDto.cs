namespace ExpenseTrackerSharedCL.Features.Transactions.Dtos
{
    public class TransactionsPageDataDto
    {
        public List<TransactionDto> Transactions { get; set; } = [];
        public int TotalItems { get; set; }
    }
}