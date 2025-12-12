using MediatR;
using FluentValidation;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;

namespace ExpenseTrackerWebApi.Features.Transactions.Queries
{
    public class GetTransactionsQuery : IRequest<TransactionsPageDataDto>
    {
        public required string UserId{get; set; }
        public required TransactionsGridOptionsDto TransactionOptions{get; set;} 
    }


    
    public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
    {
        public GetTransactionsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.TransactionOptions)
                .NotNull().WithMessage("Transaction options are required!");
        }
    }
}