
namespace ExpenseTrackerSharedCL.Features.Accounts.Dtos
{
    public class AccountDto
    {
        public int? Id{get; set;}
        public string? Name{get; set;}
        public decimal? InitialBalance{get; set;}
        public string? Icon{get; set;}
        public string? Color{get; set;}
    }
}