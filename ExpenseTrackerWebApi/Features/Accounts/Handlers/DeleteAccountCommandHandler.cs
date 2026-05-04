using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Accounts.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Accounts.Handlers
{
    public class DeleteAccountCommandHandler(AppDbContext context) : IRequestHandler<DeleteAccountCommand>
    {
        private readonly AppDbContext _context = context;

        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts
                .Where(a => a.Id == request.Id).FirstOrDefaultAsync(cancellationToken) ?? throw new ArgumentException("Account not found!");
            if (account.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Account does not belong to user!");
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}