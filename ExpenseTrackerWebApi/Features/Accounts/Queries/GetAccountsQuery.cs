using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Accounts.Queries
{
    public class GetAccountsQuery : IRequest<List<AccountDto>>
    {
        public required string UserId { get; set; }
    }

    public class GetAccountsQueryValidator : AbstractValidator<GetAccountsQuery>
    {
        public GetAccountsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}