using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;
using Microsoft.Identity.Client;

namespace ExpenseTrackerWebApp.Features.Budgets.Queries
{
    public class GetBudgetsQuery : IRequest<List<Budget>>
    {
        public string UserId{get; set;}
    }
}