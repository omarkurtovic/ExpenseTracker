using MediatR;
using FluentValidation;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;

namespace ExpenseTrackerWebApi.Features.Transactions.Queries
{
    public class GetTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public required string UserId{get; set; }
        public required bool IsReoccuring{get; set;}
    }


    
    public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
    {
        public GetTransactionsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}