using ExpenseTrackerWebApp.Database.Models;

namespace ExpenseTrackerWebApp.Features.Categories.Dtos
{
    public class CategoryDto
    {
        public string? Name {get; set;}
        public TransactionType? Type {get; set;}
        public string? Icon {get; set;}
        public string? Color{get; set;}
    }
}