using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Transactions.Commands{
    public class SeedUserCommand : IRequest
    {
        public required string UserId {get; set;}
    }


    public class SeedUserQueryValidator : AbstractValidator<SeedUserCommand>
    {
        public SeedUserQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }

}