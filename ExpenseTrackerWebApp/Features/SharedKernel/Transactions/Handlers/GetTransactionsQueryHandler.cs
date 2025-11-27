using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Queries
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<Transaction>>
    {
        private readonly AppDbContext _context;

        public GetTransactionsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Transaction>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .Include(t => t.TransactionTags)
                .ThenInclude(tt => tt.Tag)
            .Where(t => t.Account.IdentityUserId == request.UserId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

            if (request.IsReoccuring)
            {
                result = result.Where(t => t.IsReoccuring!= null && t.IsReoccuring == true).ToList();
            }
            else
            {
                result = result.Where(t => t.IsReoccuring == null || t.IsReoccuring == false).ToList();
            }

            return result;
        }

    }
}