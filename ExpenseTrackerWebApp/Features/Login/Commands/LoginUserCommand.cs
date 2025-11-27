using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Features.Login.Commands
{
    public class LoginUserCommand : IRequest<SignInResult>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required!");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required!");
        }
    }
}
