namespace ExpenseTrackerWebApp.Features.SharedKernel.Commands
{
    using FluentValidation;
    using MediatR;

    public class ResetToDefaultAccountsCommand : IRequest
    {
        public required string UserId { get; set; }
    }

    public class ResetToDefaultAccountsCommandValidator : AbstractValidator<ResetToDefaultAccountsCommand>
    {
        public ResetToDefaultAccountsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}