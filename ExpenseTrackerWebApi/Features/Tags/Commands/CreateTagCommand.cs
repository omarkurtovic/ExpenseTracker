using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Tags.Commands
{
    public class CreateTagCommand : IRequest<int>
    {
        public required string UserId { get; set; }
        public required TagDto TagDto { get; set; }
    }

    public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.TagDto.Name)
                .NotEmpty().WithMessage("Name is required!")
                .When(x => x.TagDto != null);
        }
    }
}
