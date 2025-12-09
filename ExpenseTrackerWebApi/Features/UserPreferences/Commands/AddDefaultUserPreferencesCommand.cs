using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.UserPreferences.Commands
{
    public class AddDefaultUserPreferencesCommand : IRequest
    {
        public required string UserId { get; set; }
    }

    public class AddDefaultUserPreferencesCommandValidator : AbstractValidator<AddDefaultUserPreferencesCommand>
    {
        public AddDefaultUserPreferencesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}