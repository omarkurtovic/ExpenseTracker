using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Accounts.Fixtures
{
    public class AccountTestFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public string NoAccountsUserId { get; private set; } = null!;
        public string AccountNoTransactionsUserId { get; private set; } = null!;
        public string AccountWithTransactionsUserId { get; private set; } = null!;
        public string MultipleAccountsUserId { get; private set; } = null!;

        public AccountTestData NoAccounts { get; private set; } = null!;
        public AccountTestData AccountNoTransactions { get; private set; } = null!;
        public AccountTestData AccountWithTransactions { get; private set; } = null!;
        public AccountTestData MultipleAccounts { get; private set; } = null!;

        public AccountTestFixture()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
            
            NoAccountsUserId = Guid.NewGuid().ToString();
            AccountNoTransactionsUserId = Guid.NewGuid().ToString();
            AccountWithTransactionsUserId = Guid.NewGuid().ToString();
            MultipleAccountsUserId = Guid.NewGuid().ToString();

            var users = new[]
            {
                CreateUser(NoAccountsUserId, "noaccounts@test.com"),
                CreateUser(AccountNoTransactionsUserId, "accountnotx@test.com"),
                CreateUser(AccountWithTransactionsUserId, "accounttx@test.com"),
                CreateUser(MultipleAccountsUserId, "multiaccounts@test.com")
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            NoAccounts = new AccountTestData();
            AccountNoTransactions = CreateUserWithAccountNoTransactions(context, AccountNoTransactionsUserId);
            AccountWithTransactions = CreateUserWithAccountAndTransactions(context, AccountWithTransactionsUserId);
            MultipleAccounts = CreateUserWithMultipleAccountsAndTransactions(context, MultipleAccountsUserId);
        }

        private IdentityUser CreateUser(string id, string email)
        {
            return new IdentityUser
            {
                Id = id,
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true
            };
        }

        private AccountTestData CreateUserWithAccountNoTransactions(AppDbContext context, string userId)
        {
            var data = new AccountTestData();
            
            var account = new Account
            {
                Name = "Checking Account",
                InitialBalance = 1000m,
                Icon = "account_balance",
                Color = "#FF6B6B",
                IdentityUserId = userId
            };

            context.Accounts.Add(account);
            context.SaveChanges();

            data.AccountIds.Add(account.Id);

            return data;
        }

        private AccountTestData CreateUserWithAccountAndTransactions(AppDbContext context, string userId)
        {
            var data = CreateUserWithAccountNoTransactions(context, userId);

            var category = new Category
            {
                Name = "Groceries",
                Type = TransactionType.Expense,
                IdentityUserId = userId
            };

            context.Categories.Add(category);
            context.SaveChanges();

            var transaction = new Transaction
            {
                AccountId = data.AccountIds[0],
                CategoryId = category.Id,
                Amount = -50m,
                Date = DateTime.Now,
                Description = "Grocery shopping"
            };

            context.Transactions.Add(transaction);
            context.SaveChanges();

            data.TransactionIds.Add(transaction.Id);

            return data;
        }

        private AccountTestData CreateUserWithMultipleAccountsAndTransactions(AppDbContext context, string userId)
        {
            var data = new AccountTestData();

            // Create multiple accounts
            var account1 = new Account
            {
                Name = "Checking Account",
                InitialBalance = 1000m,
                Icon = "account_balance",
                Color = "#FF6B6B",
                IdentityUserId = userId
            };

            var account2 = new Account
            {
                Name = "Savings Account",
                InitialBalance = 5000m,
                Icon = "savings",
                Color = "#4ECDC4",
                IdentityUserId = userId
            };

            var account3 = new Account
            {
                Name = "Credit Card",
                InitialBalance = 0m,
                Icon = "credit_card",
                Color = "#95E1D3",
                IdentityUserId = userId
            };

            context.Accounts.AddRange(account1, account2, account3);
            context.SaveChanges();

            data.AccountIds.AddRange(account1.Id, account2.Id, account3.Id);

            // Create categories
            var expenseCategory = new Category
            {
                Name = "Food",
                Type = TransactionType.Expense,
                IdentityUserId = userId
            };

            var incomeCategory = new Category
            {
                Name = "Salary",
                Type = TransactionType.Income,
                IdentityUserId = userId
            };

            context.Categories.AddRange(expenseCategory, incomeCategory);
            context.SaveChanges();

            // Create transactions for Account 1 (Checking)
            var tx1 = new Transaction
            {
                AccountId = account1.Id,
                CategoryId = expenseCategory.Id,
                Amount = -75m,
                Date = DateTime.Now,
                Description = "Restaurant"
            };

            var tx2 = new Transaction
            {
                AccountId = account1.Id,
                CategoryId = expenseCategory.Id,
                Amount = -30m,
                Date = DateTime.Now,
                Description = "Coffee"
            };

            // Create transactions for Account 2 (Savings)
            var tx3 = new Transaction
            {
                AccountId = account2.Id,
                CategoryId = incomeCategory.Id,
                Amount = 2000m,
                Date = DateTime.Now,
                Description = "Interest"
            };

            // Create transactions for Account 3 (Credit Card)
            var tx4 = new Transaction
            {
                AccountId = account3.Id,
                CategoryId = expenseCategory.Id,
                Amount = -150m,
                Date = DateTime.Now,
                Description = "Online purchase"
            };

            context.Transactions.AddRange(tx1, tx2, tx3, tx4);
            context.SaveChanges();

            data.TransactionIds.AddRange(tx1.Id, tx2.Id, tx3.Id, tx4.Id);

            return data;
        }

        public AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }

    public class AccountTestData
    {
        public List<int> AccountIds { get; } = new();
        public List<int> TransactionIds { get; } = new();
    }
}
