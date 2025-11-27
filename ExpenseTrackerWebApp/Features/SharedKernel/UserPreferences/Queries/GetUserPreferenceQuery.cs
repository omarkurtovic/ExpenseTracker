using ExpenseTrackerWebApp.Database.Models;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Queries
{
    public class GetUserPreferencesQuery : IRequest<UserPreference?>
    {
    }
}
