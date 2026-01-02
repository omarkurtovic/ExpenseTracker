using MediatR;
using FluentValidation;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;

namespace ExpenseTrackerWebApi.Features.Tags.Queries
{
    public class GetTagsQuery : IRequest<List<TagDto>>
    {
        public required string UserId{get; set; }
    }

    
    public class GetTagsQueryValidator : AbstractValidator<GetTagsQuery>
    {
        public GetTagsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}