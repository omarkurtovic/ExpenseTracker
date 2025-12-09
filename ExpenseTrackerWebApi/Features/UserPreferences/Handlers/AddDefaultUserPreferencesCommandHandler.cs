

using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.UserPreferences.Commands;
using MediatR;

namespace ExpenseTrackerWebApi.Features.UserPreferences.Handlers
{
    public class AddDefaultUserPreferencesCommandHandler : IRequestHandler<AddDefaultUserPreferencesCommand>
    {
        private readonly AppDbContext _context;
        public AddDefaultUserPreferencesCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddDefaultUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            var preference = new UserPreference
            {
                UserId = request.UserId,
                DarkMode = false
            };

            _context.UserPreferences.Add(preference);
            await _context.SaveChangesAsync();
        }
    }
}