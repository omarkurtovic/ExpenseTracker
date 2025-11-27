using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Queries
{
    public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, UserPreference?>
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetUserPreferencesQueryHandler(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<UserPreference?> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            return await _context.UserPreferences
                .Where(p => p.UserId == userId)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
