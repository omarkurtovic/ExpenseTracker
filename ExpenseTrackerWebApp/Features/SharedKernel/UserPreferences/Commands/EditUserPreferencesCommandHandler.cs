using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.UserPreferences.Handlers
{
    public class EditUserPreferencesCommandHandler : IRequestHandler<EditUserPreferencesCommand>
    {
        private readonly AppDbContext _context;

        public EditUserPreferencesCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(EditUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            var preferences = await _context.UserPreferences
                .SingleAsync(p => p.UserId == request.UserId, cancellationToken);

            preferences.DarkMode = request.DarkMode;
            await _context.SaveChangesAsync(cancellationToken);
        }



    }
}
