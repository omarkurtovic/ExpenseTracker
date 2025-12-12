namespace ExpenseTrackerSharedCL.Features.Transactions.Dtos
{
    public class TransactionsPageDataDto
    {
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
        public int TotalItems { get; set; }
    }
}