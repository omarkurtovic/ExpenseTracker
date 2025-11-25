using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Budgets.Commands
{
    public class DeleteBudgetCommand : IRequest
    {
        public int Id{get; set;}
    }
}