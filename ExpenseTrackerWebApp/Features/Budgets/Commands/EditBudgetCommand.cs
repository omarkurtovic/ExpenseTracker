using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Budgets.Commands
{
    public class EditBudgetCommand : IRequest<int>
    {
        public int Id{get; set;}
        public BudgetDto BudgetDto{get; set;}
        public EditBudgetCommand()
        {
            
        }
        public EditBudgetCommand(int id, BudgetDto budgetDto)
        {
            Id = id;
            BudgetDto = budgetDto;
        }
    }
}