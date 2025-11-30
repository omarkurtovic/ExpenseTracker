namespace ExpenseTrackerWebApp.Features.Transactions.Dtos
{
    public class SeedDataOptionsDto
    {
        public int? NumberOfTransaction{get; set;}
        public int? TransactionMinAmount{get; set;}
        public int? TransactionMaxAmount{get; set;}
        public DateTime? TransactionStartDate{get; set;}
        public DateTime? TransactionEndDate{get; set;}
        public  int? MaxNumberOfTags{get; set;}

        public SeedDataOptionsDto()
        {
            NumberOfTransaction = 1000;
            TransactionMinAmount = 10;
            TransactionMaxAmount = 500;
            TransactionStartDate = DateTime.Today.AddMonths(-6);
            TransactionEndDate = DateTime.Today;
            MaxNumberOfTags = 3;
        }
    }
}