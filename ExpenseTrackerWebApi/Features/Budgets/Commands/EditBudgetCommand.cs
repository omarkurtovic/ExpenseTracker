using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Budgets.Commands
{
    public class EditBudgetCommand : IRequest<int>
    {
        public required int Id{get; set;}
        public required string UserId{get; set;}
        public required BudgetDto BudgetDto{get; set;}
    }
    
    public class EditBudgetCommandValidator : AbstractValidator<EditBudgetCommand>
    {
        public EditBudgetCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.BudgetDto)
                .NotEmpty().WithMessage("Budget is required!");

            RuleFor(x => x.BudgetDto.Name)
                .NotEmpty().WithMessage("Name is required!")
                .When(x => x.BudgetDto != null);

            RuleFor(x => x.BudgetDto.BudgetType)
                .NotNull().WithMessage("Budget type is required!")
                .When(x => x.BudgetDto != null);

            RuleFor(x => x.BudgetDto.Amount)
                .NotNull().WithMessage("Amount is required!")
                .GreaterThan(0).WithMessage("Amount must be greater than zero!")
                .When(x => x.BudgetDto != null);

            RuleFor(x => x.BudgetDto.Categories)
                .NotEmpty().WithMessage("At least one category must be selected!")
                .When(x => x.BudgetDto != null);

            RuleFor(x => x.BudgetDto.Accounts)
                .NotEmpty().WithMessage("At least one account must be selected!")
                .When(x => x.BudgetDto != null);
        }
    }
}