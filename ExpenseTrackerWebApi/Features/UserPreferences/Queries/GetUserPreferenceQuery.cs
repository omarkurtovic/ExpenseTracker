using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using FluentValidation;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Features.UserPreferences.Queries
{
    public class GetUserPreferencesQuery : IRequest<UserPreferenceDto?>
    {
        public required string UserId{get; set;}
    }

    
    public class GetUserPreferencesQueryValidator : AbstractValidator<GetUserPreferencesQuery>
    {
        public GetUserPreferencesQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required!");
        }
    }
}
