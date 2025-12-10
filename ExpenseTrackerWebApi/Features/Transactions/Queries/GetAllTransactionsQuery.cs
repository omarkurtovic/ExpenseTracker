using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Transactions.Queries
{
    public class GetAllTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public required string UserId{get; set; }
        public required bool IsReoccuring{get; set;}
    }
    
    public class GetAllTransactionsQueryValidator : AbstractValidator<GetAllTransactionsQuery>
    {
        public GetAllTransactionsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");

            RuleFor(x => x.IsReoccuring)
                .NotNull().WithMessage("IsReoccuring flag is required!");
        }
    }
}