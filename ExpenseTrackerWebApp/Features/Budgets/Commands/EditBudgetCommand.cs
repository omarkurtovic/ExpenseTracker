using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using FluentValidation;
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
    
    public class EditBudgetCommandValidator : AbstractValidator<EditBudgetCommand>
    {
        public EditBudgetCommandValidator()
        {
            RuleFor(x => x.BudgetDto.IdentityUserId)
                .NotEmpty().WithMessage("User is required!");

            RuleFor(x => x.BudgetDto.Name)
                .NotEmpty().WithMessage("Name is required!");

            RuleFor(x => x.BudgetDto.BudgetType)
                .NotNull().WithMessage("Budget type is required!");

            RuleFor(x => x.BudgetDto.Amount)
                .NotNull().WithMessage("Amount is required!")
                .GreaterThan(0).WithMessage("Amount must be greater than zero!");

            RuleFor(x => x.BudgetDto.Categories)
                .NotNull().WithMessage("At least one category must be selected!")
                .Must(c => c.Any()).WithMessage("At least one category must be selected!");

            RuleFor(x => x.BudgetDto.Accounts)
                .NotNull().WithMessage("At least one account must be selected!")
                .Must(a => a.Any()).WithMessage("At least one account must be selected!");
        }
    }
}