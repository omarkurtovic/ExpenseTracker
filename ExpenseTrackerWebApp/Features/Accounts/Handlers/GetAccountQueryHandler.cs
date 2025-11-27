

using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Accounts.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Accounts.Handlers
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
    {
        private readonly AppDbContext _context;

        public GetAccountQueryHandler(AppDbContext context)
        {
            _context = context;
        }

       public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            Account? account = await _context.Accounts
            .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            

            if(account == null)
            {
                throw new ArgumentException("Account not found!");
            }

            if(account.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Account does not belong to user!");
            }

            return new AccountDto()
            {
                Name = account.Name,
                InitialBalance = account.InitialBalance,
                Color = account.Color,
                Icon = account.Icon
            };
        }
    }
}