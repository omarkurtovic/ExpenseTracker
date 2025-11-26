using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Budgets.Commands
{
    public class CreateBudgetCommand : IRequest<int>
    {
        public required string UserId{get; set;}
        public required BudgetDto BudgetDto{get; set;}
    }

    public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
    {
        public CreateBudgetCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.BudgetDto.Name)
                .NotEmpty().WithMessage("Name is required!");

            RuleFor(x => x.BudgetDto.BudgetType)
                .NotNull().WithMessage("Budget type is required!");

            RuleFor(x => x.BudgetDto.Amount)
                .NotNull().WithMessage("Amount is required!")
                .GreaterThan(0).WithMessage("Amount must be greater than zero!");

            RuleFor(x => x.BudgetDto.Categories)
                .NotEmpty().WithMessage("At least one category must be selected!");

            RuleFor(x => x.BudgetDto.Accounts)
                .NotEmpty().WithMessage("At least one account must be selected!");
        }
    }
}