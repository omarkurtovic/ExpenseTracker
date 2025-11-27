using ExpenseTrackerWebApp.Database.Models;
using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetCategoriesQuery : IRequest<List<Category>>
    {
        public required string UserId{get; set; }
    }

    
    
    public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}