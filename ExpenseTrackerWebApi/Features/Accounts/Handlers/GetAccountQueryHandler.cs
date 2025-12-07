

using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Accounts.Handlers
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