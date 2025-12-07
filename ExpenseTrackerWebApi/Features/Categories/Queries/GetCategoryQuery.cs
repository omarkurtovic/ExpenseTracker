using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Categories.Queries
{
    public class GetCategoryQuery : IRequest<CategoryDto>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
    {
        public GetCategoryQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}
