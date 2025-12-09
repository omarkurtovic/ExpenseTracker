using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Transactions.Queries
{
    public class GetTransactionQuery : IRequest<TransactionDto>
    {
        public required int Id{get; set;}
        public required string UserId{get; set; }
    }
    
    public class GetTransactionQueryValidator : AbstractValidator<GetTransactionQuery>
    {
        public GetTransactionQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");
            
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}