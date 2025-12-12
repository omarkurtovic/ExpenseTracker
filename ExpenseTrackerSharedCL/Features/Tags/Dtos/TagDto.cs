
namespace ExpenseTrackerSharedCL.Features.Tags.Dtos
{
    public class TagDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? IdentityUserId { get; set; }
        

        public bool Equals(TagDto? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => obj is TagDto tag && Equals(tag);

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Name;
    }
}