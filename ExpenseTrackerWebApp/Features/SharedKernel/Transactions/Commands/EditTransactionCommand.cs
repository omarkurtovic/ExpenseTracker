using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands
{
    public class EditTransactionCommand : IRequest<int>
    {
        public int Id{get; set;}
        public required string UserId{get; set;}
        public required TransactionDto TransactionDto{get; set;}
    }

    public class EditTransactionCommandValidator : AbstractValidator<EditTransactionCommand>
    {
        public EditTransactionCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

            RuleFor(x => x.TransactionDto)
                .NotEmpty().WithMessage("Transaction is required!");

            RuleFor(x => x.TransactionDto.Amount)
                .NotEmpty().WithMessage("Amount is required!")
                .When(x => x.TransactionDto != null);
                
            RuleFor(x => x.TransactionDto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0!")
                .When(x => x.TransactionDto != null);

            RuleFor(x => x.TransactionDto.Date)
                .NotNull().WithMessage("Date is required!")
                .When(x => x.TransactionDto != null);

            RuleFor(x => x.TransactionDto.Time)
                .NotEmpty().WithMessage("Time is required!")
                .When(x => x.TransactionDto != null);

            RuleFor(x => x.TransactionDto.AccountId)
                .NotEmpty().WithMessage("Account ID is required!")
                .When(x => x.TransactionDto != null);

            RuleFor(x => x.TransactionDto.CategoryId)
                .NotEmpty().WithMessage("Category ID is required!")
                .When(x => x.TransactionDto != null);

            RuleFor(x => x.TransactionDto.ReoccuranceFrequency)
                .NotEmpty().WithMessage("Reoccurance Frequency is required!")
                .When(x => x.TransactionDto != null && x.TransactionDto.IsReoccuring == true);
        }
    }
}