using MediatR;
using FluentValidation;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;

namespace ExpenseTrackerWebApi.Features.Transactions.Queries
{
    public class GetTransactionsPageDataQuery : IRequest<TransactionsPageDataDto>
    {
        public required string UserId{get; set; }
        public required TransactionsGridOptionsDto TransactionOptions{get; set;} 
    }


    
    public class GetTransactionsPageDataQueryValidator : AbstractValidator<GetTransactionsPageDataQuery>
    {
        public GetTransactionsPageDataQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.TransactionOptions)
                .NotNull().WithMessage("Transaction options are required!");
        }
    }
}