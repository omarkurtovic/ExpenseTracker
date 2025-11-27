using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Commands;
using MediatR;

namespace ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Handlers
{
    public class CreateUserPreferencesCommandHandler : IRequestHandler<CreateUserPreferencesCommand>
    {
        private readonly AppDbContext _context;

        public CreateUserPreferencesCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            var preference = new UserPreference
            {
                UserId = request.UserId,
                DarkMode = request.DarkMode
            };

            _context.UserPreferences.Add(preference);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
