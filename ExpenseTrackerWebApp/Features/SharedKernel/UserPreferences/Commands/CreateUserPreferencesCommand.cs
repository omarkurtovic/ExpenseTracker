using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Commands
{
    public class CreateUserPreferencesCommand : IRequest
    {
        public required string UserId { get; set; }
        public required bool DarkMode { get; set; } = false;
    }

    public class CreateUserPreferencesCommandValidator : AbstractValidator<CreateUserPreferencesCommand>
    {
        public CreateUserPreferencesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.DarkMode)
                .NotNull().WithMessage("Dark mode preference is required!");
        }
    }
}
