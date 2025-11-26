using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Accounts.Queries
{
    public class GetAccountQuery : IRequest<AccountDto>
    {
        public required int Id{get; set;}
        public required string UserId{get; set;}
    }

    
    public class GetAccountQueryValidator : AbstractValidator<GetAccountQuery>
    {
        public GetAccountQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");
            
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}