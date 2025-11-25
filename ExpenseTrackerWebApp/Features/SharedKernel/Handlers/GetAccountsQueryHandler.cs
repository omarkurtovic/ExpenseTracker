using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<Account>>
    {
        private readonly AppDbContext _context;

        public GetAccountsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Account>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Accounts
                .Where(c => c.IdentityUserId == request.UserId)
                .ToListAsync();
        }
    }
}