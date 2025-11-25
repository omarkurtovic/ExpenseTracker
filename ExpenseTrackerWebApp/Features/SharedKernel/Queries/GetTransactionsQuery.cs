using ExpenseTrackerWebApp.Database.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetTransactionsQuery : IRequest<List<Transaction>>
    {
        public string UserId{get; set; }
        public bool IsReoccuring{get; set;} = false;
    }
}