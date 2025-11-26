using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Accounts.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Accounts.Handlers
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly AppDbContext _context;
        public DeleteAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts
                .Where(a => a.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if(account == null)
            {
                throw new ArgumentException("Account not found!");
            }

            if(account.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Account does not belong to user!");
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}