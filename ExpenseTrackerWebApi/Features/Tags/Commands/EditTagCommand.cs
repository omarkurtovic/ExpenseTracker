using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Tags.Commands
{
    public class EditTagCommand : IRequest<int>
    {
        public required int Id { get; set; }
        public required string UserId { get; set; }
        public required TagDto TagDto { get; set; }
    }

    public class EditTagCommandValidator : AbstractValidator<EditTagCommand>
    {
        public EditTagCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID is required!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.TagDto.Name)
                .NotEmpty().WithMessage("Name is required!")
                .When(x => x.TagDto != null);
        }
    }
}
