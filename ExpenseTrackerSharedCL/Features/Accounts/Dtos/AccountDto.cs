
namespace ExpenseTrackerSharedCL.Features.Accounts.Dtos
{
    public class AccountDto : IEquatable<AccountDto>
    {
        public int? Id{get; set;}
        public string? Name{get; set;}
        public decimal? InitialBalance{get; set;}
        public string? Icon{get; set;}
        public string? Color{get; set;}

        public bool Equals(AccountDto? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj) => obj is AccountDto account && Equals(account);

        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Name;
    }
}