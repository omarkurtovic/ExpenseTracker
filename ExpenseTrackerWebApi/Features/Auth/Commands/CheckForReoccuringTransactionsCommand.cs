using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApi.Features.Auth.Commands
{
    public class CheckForReoccuringTransactionsCommand : IRequest
    {
        public required string UserId{get; set;}
    }

    public class CheckForReoccuringTransactionsCommandValidator : AbstractValidator<CheckForReoccuringTransactionsCommand>
    {
        public CheckForReoccuringTransactionsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}