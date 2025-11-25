using ExpenseTrackerWebApp.Features.Budgets.Models;
using FluentValidation;
using MediatR;
using Microsoft.Identity.Client;

namespace ExpenseTrackerWebApp.Features.Budgets.Queries
{
    public class GetBudgetQuery : IRequest<Budget?>
    {
        public int Id{get; set;}
        public string UserId{get; set;}
    }
    
    public class GetBudgetQueryValidator : AbstractValidator<GetBudgetQuery>
    {
        public GetBudgetQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("Id is required!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is required!");
        }
    }
}