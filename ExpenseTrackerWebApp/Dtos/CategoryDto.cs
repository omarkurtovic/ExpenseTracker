using ExpenseTrackerWebApp.Database.Models;

namespace ExpenseTrackerWebApp.Dtos
{
    public class CategoryDto
    {
        public int? Id {get; set;}
        public string? Name {get; set;}
        public TransactionType? Type {get; set;}
        public string? UserId {get; set;}
    }
}