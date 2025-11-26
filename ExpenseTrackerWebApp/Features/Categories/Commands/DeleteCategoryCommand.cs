using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest
    {
        public required int Id { get; set; }
        public required string UserId { get; set; }
    }
    

    public class DeleteCategoryCommandHandler : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandHandler()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}