using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
        public required string UserId { get; set; }
        public TransactionTypeDto? Type { get; set; }
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
