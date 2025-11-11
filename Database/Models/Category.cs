namespace ExpenseTracker.Database.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TransactionType Type { get; set; }
        public ICollection<Transaction> Transactions{ get; set; }
    }
}