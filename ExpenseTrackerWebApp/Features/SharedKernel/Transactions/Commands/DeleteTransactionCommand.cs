using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands
{
    public class DeleteTransactionCommand : IRequest
    {
        public required int Id { get; set; }
        public required string UserId { get; set; }
    }

    public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
    {
        public DeleteTransactionCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

        }
    }
}