using ExpenseTrackerWebApp.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.Identity.Client;

namespace ExpenseTrackerWebApp.Features.Accounts.Queries
{
    public class GetAccountsWithBalanceQuery : IRequest<List<AccountWithBalanceDto>>
    {
        public string UserId { get; set; }
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