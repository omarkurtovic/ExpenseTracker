using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.UserPreferences.Commands
{
    public class EditUserPreferencesCommand : IRequest<int>
    {
        public required int Id{get; set;}
        public required string UserId{get; set;}
        public required UserPreferenceDto UserPreferencesDto{get; set;}
    }
    
    public class EditUserPreferencesCommandValidator : AbstractValidator<EditUserPreferencesCommand>
    {
        public EditUserPreferencesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.UserPreferencesDto)
                .NotEmpty().WithMessage("User preferences are required!");

            RuleFor(x => x.UserPreferencesDto.DarkMode)
                .NotNull().WithMessage("Dark mode preference is required!")
                .When(x => x.UserPreferencesDto != null);
        }
    }
}