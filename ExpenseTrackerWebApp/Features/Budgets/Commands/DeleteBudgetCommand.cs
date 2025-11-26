using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Budgets.Commands
{
    public class DeleteBudgetCommand : IRequest
    {
        public required int Id {get; set;}
        public required string UserId {get; set;}
    }
    public class DeleteBudgetCommandValidator : AbstractValidator<DeleteBudgetCommand>
    {
        public DeleteBudgetCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}