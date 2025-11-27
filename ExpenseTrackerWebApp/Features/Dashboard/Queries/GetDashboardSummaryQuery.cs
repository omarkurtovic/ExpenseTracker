using ExpenseTrackerWebApp.Features.Dashboard.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Dashboard.Queries
{
    public class GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
    {
        public required string UserId{get; set;}
        public int MonthsBehindToConsider{get; set;} = 5;
        public int MaxCategoriesToShow {get; set;} = 6;
    }
    public class GetDashboardSummaryQueryValidator : AbstractValidator<GetDashboardSummaryQuery>
    {
        public GetDashboardSummaryQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}