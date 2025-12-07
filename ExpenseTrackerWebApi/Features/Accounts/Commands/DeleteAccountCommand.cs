using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApi.Features.Accounts.Commands
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
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}