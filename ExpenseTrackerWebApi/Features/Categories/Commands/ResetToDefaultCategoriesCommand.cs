using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Categories.Commands
{
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