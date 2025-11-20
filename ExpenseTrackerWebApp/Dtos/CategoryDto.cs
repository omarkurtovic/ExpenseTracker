using ExpenseTrackerWebApp.Database.Models;

namespace ExpenseTrackerWebApp.Dtos
{
    public class CategoryDto
    {
        public int? Id {get; set;}
        public string? Name {get; set;}
        public TransactionType? Type {get; set;}
        public string? Icon {get; set;}
        public string? Color{get; set;}
        public string? UserId {get; set;}
    }
}