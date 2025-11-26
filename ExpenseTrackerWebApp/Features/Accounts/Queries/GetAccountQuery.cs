using ExpenseTrackerWebApp.Database.Models;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Accounts.Queries
{
    public class GetAccountQuery : IRequest<Account>
    {
        public int Id{get; set;}
        public string UserId{get; set;}
    }

    
    public class GetAccountQueryValidator : AbstractValidator<GetAccountQuery>
    {
        public GetAccountQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");
            
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}