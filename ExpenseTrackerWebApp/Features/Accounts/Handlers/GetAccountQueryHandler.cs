

using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Accounts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Accounts.Handlers
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Account>
    {
        private readonly AppDbContext _context;

        public GetAccountQueryHandler(AppDbContext context)
        {
            _context = context;
        }

       public async Task<Account> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            Account? account = await _context.Accounts
            .Where(b => b.Id == request.Id).FirstOrDefaultAsync();
            

            if(account == null)
            {
                throw new ArgumentException("Account not found!");
            }

            if(account.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Account does not belong to user!");
            }

            return account;
        }
    }
}