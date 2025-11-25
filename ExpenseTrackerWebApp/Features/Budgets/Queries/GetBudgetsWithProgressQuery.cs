using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using FluentValidation;
using MediatR;
using Microsoft.Identity.Client;

namespace ExpenseTrackerWebApp.Features.Budgets.Queries
{
    public class GetBudgetsWithProgressQuery : IRequest<List<BudgetWithProgressDto>>
    {
        public required string UserId { get; set; }
    }

    public class GetBudgetsWithProgressQueryValidator : AbstractValidator<GetBudgetsWithProgressQuery>
    {
        public GetBudgetsWithProgressQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is required!");
        }
    }
}