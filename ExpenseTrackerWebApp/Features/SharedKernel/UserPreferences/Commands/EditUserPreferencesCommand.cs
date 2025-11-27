using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Commands
{
    public class EditUserPreferencesCommand : IRequest
    {
        public required string UserId { get; set; }
        public required bool DarkMode { get; set; }
    }

    public class EditUserPreferencesCommandValidator : AbstractValidator<EditUserPreferencesCommand>
    {
        public EditUserPreferencesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
                
            RuleFor(x => x.DarkMode)
                .NotNull().WithMessage("Dark mode preference is required!");
        }
    }
}
