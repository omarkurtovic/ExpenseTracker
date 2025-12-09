using ExpenseTrackerSharedCL.Features.Transactions.Dtos;

namespace ExpenseTrackerSharedCL.Features.Tags.Dtos
{
    public class TransactionTagDto
    {
        public int TransactionId { get; set; }
        public TransactionDto Transaction { get; set; } = null!;

        public int TagId { get; set; }
        public TagDto Tag { get; set; } = null!;
    }
}