using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApp.Features.Accounts.Commands
{
    public class DeleteAccountCommand : IRequest
    {
        public required string UserId{get; set;}
        public required int Id{get; set;}
    }

    public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
    {
        public DeleteAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("Id is required!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}