using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Tags.Commands
{
    public class DeleteTagCommand : IRequest
    {
        public required int Id { get; set; }
        public required string UserId { get; set; }
    }

    public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
    {
        public DeleteTagCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}
