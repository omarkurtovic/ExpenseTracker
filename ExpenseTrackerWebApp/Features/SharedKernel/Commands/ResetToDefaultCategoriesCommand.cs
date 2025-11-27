namespace ExpenseTrackerWebApp.Features.SharedKernel.Commands
{
    using FluentValidation;
    using MediatR;

    public class ResetToDefaultCategoriesCommand : IRequest
    {
        public required string UserId { get; set; }
    }

    public class ResetToDefaultCategoriesCommandValidator : AbstractValidator<ResetToDefaultCategoriesCommand>
    {
        public ResetToDefaultCategoriesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}