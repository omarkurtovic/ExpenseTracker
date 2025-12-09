using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Tags.Commands
{

    public class ResetToDefaultTagsCommand : IRequest
    {
        public required string UserId { get; set; }
    }

    public class ResetToDefaultTagsCommandValidator : AbstractValidator<ResetToDefaultTagsCommand>
    {
        public ResetToDefaultTagsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}