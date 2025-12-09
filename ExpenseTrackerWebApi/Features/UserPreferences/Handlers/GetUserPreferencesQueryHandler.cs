using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Features.UserPreferences.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.SharedKernel.UserPreferences.Queries
{
    public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, UserPreferenceDto?>
    {
        private readonly AppDbContext _context;

        public GetUserPreferencesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserPreferenceDto?> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            return await _context.UserPreferences
                .Where(p => p.UserId == userId)
                .Select(p => new UserPreferenceDto
                {
                    UserId = p.UserId,
                    DarkMode = p.DarkMode
                })
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
