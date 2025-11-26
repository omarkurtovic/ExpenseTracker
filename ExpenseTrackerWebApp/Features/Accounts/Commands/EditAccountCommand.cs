using ExpenseTrackerWebApp.Dtos;
using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApp.Features.Accounts.Commands{
    public class EditAccountCommand : IRequest<int>
    {
        public required int Id{get; set;}
        public required string UserId{get; set;}
        public required AccountDto AccountDto{get; set;}
    }
    
    public class EditAccountCommandValidator : AbstractValidator<EditAccountCommand>
    {
        public EditAccountCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.AccountDto)
                .NotEmpty().WithMessage("Account is required!");

            RuleFor(x => x.AccountDto.Name)
                .NotEmpty().WithMessage("Name is required!");

            RuleFor(x => x.AccountDto.InitialBalance)
                .NotNull().WithMessage("Initial balance is required!");

            RuleFor(x => x.AccountDto.Icon)
                .NotEmpty().WithMessage("Icon is required!");

            RuleFor(x => x.AccountDto.Color)
                .NotEmpty().WithMessage("Icon color is required!");
        }
    }
}