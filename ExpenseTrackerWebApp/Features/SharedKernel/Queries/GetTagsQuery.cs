using ExpenseTrackerWebApp.Database.Models;
using MediatR;
using FluentValidation;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetTagsQuery : IRequest<List<Tag>>
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