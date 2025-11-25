using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;
using Microsoft.Identity.Client;

namespace ExpenseTrackerWebApp.Features.Budgets.Queries
{
    public class GetBudgetQuery : IRequest<Budget?>
    {
        public int Id{get; set;}
    }
}