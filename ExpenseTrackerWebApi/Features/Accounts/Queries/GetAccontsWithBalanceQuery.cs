using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Accounts.Queries
{
    public class GetAccountsWithBalanceQuery : IRequest<List<AccountWithBalanceDto>>
    {
        public required string UserId { get; set; }
    }

    public class GetAccountsWithBalanceQueryValidator : AbstractValidator<GetAccountsWithBalanceQuery>
    {
        public GetAccountsWithBalanceQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}