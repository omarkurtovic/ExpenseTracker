using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApi.Database.Models
{
    public class Category : IEquatable<Category>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TransactionType Type { get; set; } 
        public string? Icon{get; set;}
        public string? Color{get; set;}
        public ICollection<Transaction> Transactions{ get; set; }
        public string IdentityUserId {get; set;}
        public IdentityUser IdentityUser { get; set; }
        public ICollection<BudgetCategory> BudgetCategories { get; set; }

        public Category()
        {
            Name = "";
            Type = TransactionType.Expense;
            Transactions = [];
            BudgetCategories = [];
            IdentityUserId = "";
            IdentityUser = null!;
        }

        
        public bool Equals(Category? other)
        {if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => obj is Category category && Equals(category);

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Name;
    }
}