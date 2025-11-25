using ExpenseTrackerWebApp.Features.Budgets.Models;
using FluentValidation;
using MediatR;
using Microsoft.Identity.Client;

namespace ExpenseTrackerWebApp.Features.Budgets.Queries
{
    public class GetBudgetsQuery : IRequest<List<Budget>>
    {
        public string UserId{get; set;}
    }

    
    public class GetBudgetsQueryValidator : AbstractValidator<GetBudgetsQuery>
    {
        public GetBudgetsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}