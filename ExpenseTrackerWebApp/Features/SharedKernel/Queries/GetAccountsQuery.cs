using ExpenseTrackerWebApp.Database.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetAccountsQuery : IRequest<List<Account>>
    {
        public required string UserId{get; set; }
    }
}