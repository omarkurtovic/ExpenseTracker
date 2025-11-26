using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApp.Features.Categories.Queries
{
    public class GetCategoriesWithStatsQuery : IRequest<List<CategoryWithStatsDto>>
    {
        public required string UserId{get; set;}
        public TransactionType? Type{get; set;}
    }

    
    public class GetCategoriesWithStatsQueryValidator : AbstractValidator<GetCategoriesWithStatsQuery>
    {
        public GetCategoriesWithStatsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}