using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Budgets.Queries
{
    public class GetBudgetsQuery : IRequest<List<BudgetDto>>
    {
        public required string UserId{get; set;}
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