
namespace ExpenseTrackerSharedCL.Features.Categories.Dtos
{
    public class CategoryDto
    {
        public int? Id{get; set;}
        public string? Name {get; set;}
        public TransactionTypeDto? Type {get; set;}
        public string? Icon {get; set;}
        public string? Color{get; set;}

    }
}