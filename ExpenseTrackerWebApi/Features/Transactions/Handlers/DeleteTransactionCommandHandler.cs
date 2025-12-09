using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Transactions.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Handlers
{
    public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand>
    {
        private readonly AppDbContext _context;
        public DeleteTransactionCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.Include(t => t.Category)
                .Where(a => a.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if(transaction == null)
            {
                throw new ArgumentException("Transaction not found!");
            }

            if(transaction.Category.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Transaction does not belong to user!");
            }

            await _context.Transactions.Where(t => t.Id == request.Id).ExecuteDeleteAsync();
        }
    }
}