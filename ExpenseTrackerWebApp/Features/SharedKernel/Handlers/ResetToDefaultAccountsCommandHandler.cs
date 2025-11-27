using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Handlers
{
    public class ResetToDefaultAccountsCommandHandler : IRequestHandler<ResetToDefaultAccountsCommand>
    {
        private readonly AppDbContext _context;

        public ResetToDefaultAccountsCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(ResetToDefaultAccountsCommand request, CancellationToken cancellationToken)
        {
            await _context.Accounts.Where(c => c.IdentityUserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);
            
            _context.ChangeTracker.Clear();

            var accounts = new List<Account>()
            {
                new()
                {
                    Name = "Cash",
                    InitialBalance = 0,
                    Icon = Icons.Material.Filled.Payments,
                    Color = "#FF5733",
                    IdentityUserId = request.UserId
                },
                new()
                {
                    Name = "Bank",
                    InitialBalance = 0,
                    Icon = Icons.Material.Filled.AccountBalance,
                    Color = "#33C1FF",
                    IdentityUserId = request.UserId
                }
            };
            for(int i = 1; i <= accounts.Count; i++)
            {
                accounts[i - 1].Id = i;
            }

            _context.Accounts.AddRange(accounts);
            await _context.SaveChangesAsync();
        }
    }
}