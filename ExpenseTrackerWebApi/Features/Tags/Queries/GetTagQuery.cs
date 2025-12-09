using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Tags.Queries
{
    public class GetTagQuery : IRequest<TagDto>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class GetTagQueryValidator : AbstractValidator<GetTagQuery>
    {
        public GetTagQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero!");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}
