using ExpenseTrackerWebApp.Features.Register.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Register.Commands{
    public class RegisterNewUserCommand : IRequest<Unit>
    {
        public required RegisterDataDto RegisterDataDto {get; set;} 
    }

    public class RegisterNewUserCommandValidator : AbstractValidator<RegisterNewUserCommand>
    {
        public RegisterNewUserCommandValidator()
        {
            RuleFor(x => x.RegisterDataDto)
                .NotEmpty().WithMessage("Account is required!");

            RuleFor(x => x.RegisterDataDto.Email)
                .NotEmpty().WithMessage("Email is required!")
                .EmailAddress().WithMessage("Invalid email format!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.Password)
                .NotEmpty().WithMessage("Password is required!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.PasswordConfirm)
                .Equal(x => x.RegisterDataDto.Password).WithMessage("Passwords do not match!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.Password)
                .Must(HasMinimumLength).WithMessage("Password must be at least 6 characters!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.Password)
                .Must(HasUpperCase).WithMessage("Password must contain at least one uppercase letter!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.Password)
                .Must(HasLowerCase).WithMessage("Password must contain at least one lowercase letter!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.Password)
                .Must(HasDigit).WithMessage("Password must contain at least one digit!")
                .When(x => x.RegisterDataDto != null);

            RuleFor(x => x.RegisterDataDto.Password)
                .Must(HasNonLetterDigit).WithMessage("Password must contain at least one special character!")
                .When(x => x.RegisterDataDto != null);
        }

        private bool HasMinimumLength(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }

        private bool HasUpperCase(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);
        }

        private bool HasLowerCase(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Any(char.IsLower);
        }

        private bool HasDigit(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);
        }

        private bool HasNonLetterDigit(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Any(c => !char.IsLetterOrDigit(c));
        }
    }
}