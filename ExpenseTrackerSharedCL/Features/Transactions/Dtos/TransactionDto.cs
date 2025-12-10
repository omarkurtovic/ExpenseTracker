
using System.Globalization;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;

namespace ExpenseTrackerSharedCL.Features.Transactions.Dtos
{
    public class TransactionDto
    {
        public int? Id{get; set;}
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public DateTime? Date{get; set;}
        public string DateStr => Date!.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        public TimeSpan? Time{get; set;}
        public int? AccountId { get; set; }
        public AccountDto? AccountDto{get; set;}
        public int? CategoryId {get; set;}
        public CategoryDto? CategoryDto{get; set;}
        public TransactionTypeDto? TransactionType{get; set;}
        public List<int> TagIds { get; set; } = new List<int>();
        public List<TransactionTagDto> TransactionTags { get; set; } = new List<TransactionTagDto>();
        public bool? IsReoccuring{get; set;}
        public ReoccuranceFrequencyDto? ReoccuranceFrequency{get; set;}
        public DateTime? NextReoccuranceDate{get; set;}
    }


    public enum ReoccuranceFrequencyDto
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}