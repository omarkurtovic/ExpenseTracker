using Database;
using ExpenseTracker.Database.Models;
using ExpenseTracker.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class AccountService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        
        public AccountService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<AccountWithBalance>> GetAllWithBalanceAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Accounts.Include(a => a.Transactions)
            .Select(a => new AccountWithBalance()
            {
                Id = a.Id,
                Name = a.Name,
                InitialBalance = a.InitialBalance,
                CurrentBalance = a.InitialBalance + a.Transactions.Sum(t => t.Amount),
            }).ToListAsync();
        }

        public async Task<List<Account>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Accounts.ToListAsync();
        }

        public async Task<Account> GetAsync(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Accounts.SingleAsync(a => a.Id == accountId);
        }
        public async Task<AccountWithBalance> GetWithBalanceAsync(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Accounts
            .Include(a => a.Transactions)
            .Select(a => new AccountWithBalance()
            {
                Id = a.Id,
                Name = a.Name,
                InitialBalance = a.InitialBalance,
                CurrentBalance = a.InitialBalance + a.Transactions.Sum(t => t.Amount),
            }).SingleAsync(a => a.Id == accountId);
        }

        public async Task SaveAsync(Account account)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (account.Id == 0)
            {
                await context.Accounts.AddAsync(account);
            }
            else
            {
                var oldAccount = await context.Accounts.SingleAsync(a => a.Id == account.Id);
                if (oldAccount != null)
                {
                    context.Entry(oldAccount).CurrentValues.SetValues(account);
                }
            }
            await context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Transactions
                .Where(t => t.AccountId == accountId)
                .ExecuteDeleteAsync();

            await context.Accounts
                .Where(a => a.Id == accountId)
                .ExecuteDeleteAsync();
        }
        
        public async Task DeleteAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Transactions
                .ExecuteDeleteAsync();
                
            await context.Accounts
                .ExecuteDeleteAsync();
        }
    }
}