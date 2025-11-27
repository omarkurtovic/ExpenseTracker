using ExpenseTrackerWebApp.Database.Models;
using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Queries
{
    public class GetTransactionsQuery : IRequest<List<Transaction>>
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