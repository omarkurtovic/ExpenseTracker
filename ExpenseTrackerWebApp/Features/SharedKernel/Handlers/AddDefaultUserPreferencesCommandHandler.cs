using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;
using MediatR;
using ExpenseTrackerWebApp.Database.Models;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Handlers
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