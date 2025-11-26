using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Accounts.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Accounts.Handlers
{
    public class GetAccountsWithBalanceQueryHandler : IRequestHandler<GetAccountsWithBalanceQuery, List<AccountWithBalanceDto>>
    {
        private readonly AppDbContext _context;
        public GetAccountsWithBalanceQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        
        public Task<List<AccountWithBalanceDto>> Handle(GetAccountsWithBalanceQuery request, CancellationToken cancellationToken)
        {
            return _context.Accounts.Where(a => a.IdentityUserId == request.UserId)
            .Include(a => a.Transactions)
            .Select(a => new AccountWithBalanceDto()
            {
                Id = a.Id,
                Name = a.Name,
                InitialBalance = a.InitialBalance,
                CurrentBalance = a.InitialBalance + a.Transactions.Sum(t => t.Amount),
                Icon = a.Icon,
                Color = a.Color
            }).ToListAsync();
        }
    }
}