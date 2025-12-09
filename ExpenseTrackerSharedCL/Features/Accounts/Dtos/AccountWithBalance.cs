
namespace ExpenseTrackerSharedCL.Features.Accounts.Dtos
{
    public class AccountWithBalanceDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? UserId{get; set;}
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }
}