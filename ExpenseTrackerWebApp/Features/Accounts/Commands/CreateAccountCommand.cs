using ExpenseTrackerWebApp.Features.Accounts.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<int>
    {
        public required string UserId{get;set;}
        public required AccountDto AccountDto{get; set;}
    }

    
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.AccountDto)
                .NotEmpty().WithMessage("Account is required!");

            RuleFor(x => x.AccountDto.Name)
                .NotEmpty().WithMessage("Name is required!")
                .When(x => x.AccountDto != null);

            RuleFor(x => x.AccountDto.InitialBalance)
                .NotNull().WithMessage("Initial balance is required!")
                .When(x => x.AccountDto != null);

            RuleFor(x => x.AccountDto.Icon)
                .NotEmpty().WithMessage("Icon is required!")
                .When(x => x.AccountDto != null);

            RuleFor(x => x.AccountDto.Color)
                .NotEmpty().WithMessage("Icon color is required!")
                .When(x => x.AccountDto != null);
        }
    }
}