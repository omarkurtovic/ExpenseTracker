using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Budgets.Queries
{
    public class GetBudgetQuery : IRequest<BudgetDto>
    {
        public required int Id{get; set;}
        public required string UserId{get; set;}
    }
    
    public class GetBudgetQueryValidator : AbstractValidator<GetBudgetQuery>
    {
        public GetBudgetQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}