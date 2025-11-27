namespace ExpenseTrackerWebApp.Features.SharedKernel.Commands
{
    using FluentValidation;
    using MediatR;

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