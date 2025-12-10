
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ExpenseTrackerWebApi.Database.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        [NotMapped]
        public string DateStr => Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int CategoryId{ get; set; }
        public Category Category{ get; set; }
        public ICollection<TransactionTag> TransactionTags { get; set; }
        public bool? IsReoccuring{get; set;}
        public ReoccuranceFrequency? ReoccuranceFrequency{get; set;}
        public DateTime? NextReoccuranceDate{get; set;}

        public Transaction()
        {
            TransactionTags = new List<TransactionTag>();
            Account = null!;
            Category = null!;
        }
    }


    public enum ReoccuranceFrequency
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}