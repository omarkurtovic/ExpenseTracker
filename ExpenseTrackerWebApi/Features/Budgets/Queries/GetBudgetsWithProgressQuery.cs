using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Budgets.Queries
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
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}