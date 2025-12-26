using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using ExpenseTrackerSharedCL.Features.UserPreferences.Services;
using ExpenseTrackerWebApi.Features.Features.UserPreferences.Queries;
using ExpenseTrackerWebApi.Features.UserPreferences.Commands;
using MediatR;
using System.Security.Claims;

namespace ExpenseTrackerWebApi.Features.UserPreferences.Services
{
    public class UserPreferenceServiceServer : IUserPreferenceService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserPreferenceServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> CreateDefaultUsrPreferencesAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var command = new AddDefaultUserPreferencesCommand
                {
                    UserId = userId,
                };
                await _mediator.Send(command);
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user preferences: {ex.Message}");
                return Result.Failure("An error occurred while creating user preferences.");
            }
        }

        public async Task<Result<UserPreferenceDto>> GetUserPreferencesAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userPreferences = await _mediator.Send(new GetUserPreferencesQuery() { UserId = userId });
                if (userPreferences == null)
                {
                    return Result<UserPreferenceDto>.Failure("User preferences not found.", FailureReason.NotFound);
                }
                return Result<UserPreferenceDto>.Success(userPreferences);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user preferences: {ex.Message}");
                return Result<UserPreferenceDto>.Failure("An error occurred while fetching user preferences.");
            }
        }

        public Task<Result> UpdateUserPreferencesAsync(UserPreferenceDto preferenceDto)
        {
            throw new NotImplementedException();
        }
    }
}
