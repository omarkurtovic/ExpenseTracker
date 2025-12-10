using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Budgets.Commands;
using ExpenseTrackerWebApi.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using ExpenseTrackerWebApi.Features.UserPreferences.Commands;

namespace ExpenseTrackerWebApi.Features.UserPreferences.Handlers
{
    public class EditUserPreferencesCommandHandler : IRequestHandler<EditUserPreferencesCommand, Unit>
    {
        private readonly AppDbContext _context;

        public EditUserPreferencesCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(EditUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            var oldUserPreference = await _context.UserPreferences
                .Where(up => up.UserId == request.UserId)
                .FirstOrDefaultAsync(cancellationToken); 

            if(oldUserPreference == null)
            {
                throw new ArgumentException("User preferences not found!");
            }

            if(oldUserPreference.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("User preferences do not belong to user!");
            }

            var userPreference = new UserPreference()
            {
                UserId = request.UserId,
                DarkMode = request.UserPreferencesDto.DarkMode
            };

            oldUserPreference.DarkMode = userPreference.DarkMode;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}