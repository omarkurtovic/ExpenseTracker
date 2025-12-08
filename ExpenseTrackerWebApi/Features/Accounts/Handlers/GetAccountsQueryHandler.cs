using ExpenseTrackerWebApi.Database;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Accounts.Handlers
{
    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
    {
        private readonly AppDbContext _context;
        public GetAccountsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        
        public Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            return _context.Accounts.Where(a => a.IdentityUserId == request.UserId)
            .Include(a => a.Transactions)
            .Select(a => new AccountDto()
            {
                Id = a.Id,
                Name = a.Name,
                InitialBalance = a.InitialBalance,
                Icon = a.Icon,
                Color = a.Color
            }).ToListAsync();
        }
    }
}