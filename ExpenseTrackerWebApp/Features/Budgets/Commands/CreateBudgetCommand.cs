using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Budgets.Commands
{
    public class CreateBudgetCommand : IRequest<int>
    {
        public BudgetDto BudgetDto{get; set;}
        public CreateBudgetCommand(){}
        public CreateBudgetCommand(BudgetDto budgetDto)
        {
            BudgetDto = budgetDto;
        }

    }
}