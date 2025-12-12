
namespace ExpenseTrackerSharedCL.Features.Categories.Dtos
{
    public class CategoryDto
    {
        public int? Id{get; set;}
        public string? Name {get; set;}
        public TransactionTypeDto? Type {get; set;}
        public string? Icon {get; set;}
        public string? Color{get; set;}

        public bool Equals(CategoryDto? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => obj is CategoryDto category && Equals(category);

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Name;
    }
}