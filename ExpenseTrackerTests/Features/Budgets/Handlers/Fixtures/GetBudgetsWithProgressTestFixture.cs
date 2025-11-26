using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Budgets.Handlers.Fixtures
{
    public class GetBudgetsWithProgressTestFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public string NoDataUserId { get; private set; }
        public string NoBudgetsUserId { get; private set; }
        public string BudgetsNoTransactionsUserId { get; private set; }
        public string BudgetsWithTransactionsUserId { get; private set; }
        public string MultipleBudgetsUserId { get; private set; }
        public string RecurringTransactionUserId { get; private set; }
        public string OutOfPeriodTransactionUserId { get; private set; }

        public BudgetTestData NoData { get; private set; }
        public BudgetTestData NoBudgets { get; private set; }
        public BudgetTestData BudgetsNoTransactions { get; private set; }
        public BudgetTestData BudgetsWithTransactions { get; private set; }
        public BudgetTestData MultipleBudgets { get; private set; }
        public BudgetTestData RecurringTransactions { get; private set; }
        public BudgetTestData OutOfPeriodTransactions { get; private set; }

        public GetBudgetsWithProgressTestFixture()
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
            
            NoDataUserId = Guid.NewGuid().ToString();
            NoBudgetsUserId = Guid.NewGuid().ToString();
            BudgetsNoTransactionsUserId = Guid.NewGuid().ToString();
            BudgetsWithTransactionsUserId = Guid.NewGuid().ToString();
            MultipleBudgetsUserId = Guid.NewGuid().ToString();
            RecurringTransactionUserId = Guid.NewGuid().ToString();
            OutOfPeriodTransactionUserId = Guid.NewGuid().ToString();

            var users = new[]
            {
                CreateUser(NoDataUserId, "nodata@test.com"),
                CreateUser(NoBudgetsUserId, "nobudgets@test.com"),
                CreateUser(BudgetsNoTransactionsUserId, "budgetsnotx@test.com"),
                CreateUser(BudgetsWithTransactionsUserId, "budgetstx@test.com"),
                CreateUser(MultipleBudgetsUserId, "multibudgets@test.com"),
                CreateUser(RecurringTransactionUserId, "recurring@test.com"),
                CreateUser(OutOfPeriodTransactionUserId, "outofperiod@test.com")
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            NoData = new BudgetTestData();
            NoBudgets = CreateUserWithNoBudgetsNoTransactions(context, NoBudgetsUserId);
            BudgetsNoTransactions = CreateUserWithBudgetsNoTransactions(context, BudgetsNoTransactionsUserId);
            BudgetsWithTransactions = CreateUserWithBudgetsAndTransactions(context, BudgetsWithTransactionsUserId);
            MultipleBudgets = CreateUserWithMultipleBudgetsComplex(context, MultipleBudgetsUserId);
            RecurringTransactions = CreateUserWithRecurringTransactions(context, RecurringTransactionUserId);
            OutOfPeriodTransactions = CreateUserWithOutOfPeriodTransactions(context, OutOfPeriodTransactionUserId);

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

        private BudgetTestData CreateUserWithNoBudgetsNoTransactions(AppDbContext context, string userId)
        {
            var data = new BudgetTestData();
            var account = new Account { Name = "Test Account", InitialBalance = 1000m, IdentityUserId = userId };
            var category = new Category { Name = "Test Category", Type = TransactionType.Expense, IdentityUserId = userId };

            context.Accounts.Add(account);
            context.Categories.Add(category);
            context.SaveChanges();

            data.AccountId = account.Id;
            data.CategoryId = category.Id;

            return data;
        }

        private BudgetTestData CreateUserWithBudgetsNoTransactions(AppDbContext context, string userId)
        {
            var data = new BudgetTestData();

            var account = new Account { Name = "Test Account", InitialBalance = 1000m, IdentityUserId = userId };
            var category = new Category { Name = "Test Category", Type = TransactionType.Expense, IdentityUserId = userId };

            context.Accounts.Add(account);
            context.Categories.Add(category);
            context.SaveChanges();

            data.AccountId = account.Id;
            data.CategoryId = category.Id;

            var budget = new Budget
            {
                Name = "Test Budget",
                Amount = 500m,
                BudgetType = BudgetType.Monthly,
                IdentityUserId = userId
            };

            context.Budgets.Add(budget);
            context.SaveChanges();

            data.BudgetIds.Add(budget.Id);

            context.BudgetCategories.Add(new BudgetCategory { BudgetId = budget.Id, CategoryId = category.Id });
            context.BudgetAccounts.Add(new BudgetAccount { BudgetId = budget.Id, AccountId = account.Id });
            context.SaveChanges();

            return data;
        }

        private BudgetTestData CreateUserWithBudgetsAndTransactions(AppDbContext context, string userId)
        {
            var data = CreateUserWithBudgetsNoTransactions(context, userId);

            var transaction = new Transaction
            {
                AccountId = data.AccountId,
                CategoryId = data.CategoryId,
                Amount = 100m,
                Date = DateTime.Now,
                Description = "In-period transaction"
            };

            context.Transactions.Add(transaction);
            context.SaveChanges();

            data.TransactionIds.Add(transaction.Id);

            return data;
        }

        private BudgetTestData CreateUserWithMultipleBudgetsComplex(AppDbContext context, string userId)
        {
            var data = new BudgetTestData();

            var account1 = new Account { Name = "Account 1", InitialBalance = 1000m, IdentityUserId = userId };
            var account2 = new Account { Name = "Account 2", InitialBalance = 1000m, IdentityUserId = userId };
            var category1 = new Category { Name = "Category 1", Type = TransactionType.Expense, IdentityUserId = userId };
            var category2 = new Category { Name = "Category 2", Type = TransactionType.Expense, IdentityUserId = userId };

            context.Accounts.AddRange(account1, account2);
            context.Categories.AddRange(category1, category2);
            context.SaveChanges();

            data.AccountIds.AddRange(account1.Id, account2.Id);
            data.CategoryIds.AddRange(category1.Id, category2.Id);

            // Budget 1: Monthly, $500, 49% spent (Green)
            var budget1 = new Budget
            {
                Name = "Monthly Budget 1",
                Amount = 500m,
                BudgetType = BudgetType.Monthly,
                IdentityUserId = userId
            };

            context.Budgets.Add(budget1);
            context.SaveChanges();
            data.BudgetIds.Add(budget1.Id);

            context.BudgetCategories.Add(new BudgetCategory { BudgetId = budget1.Id, CategoryId = category1.Id });
            context.BudgetAccounts.Add(new BudgetAccount { BudgetId = budget1.Id, AccountId = account1.Id });
            context.SaveChanges();

            var tx1 = new Transaction
            {
                AccountId = account1.Id,
                CategoryId = category1.Id,
                Amount = 245m,
                Date = DateTime.Now,
                Description = "Transaction 1"
            };
            context.Transactions.Add(tx1);
            context.SaveChanges();
            data.TransactionIds.Add(tx1.Id);

            // Budget 2: Weekly, $500, 99% spent (Error)
            var budget2 = new Budget
            {
                Name = "Weekly Budget",
                Amount = 500m,
                BudgetType = BudgetType.Weekly,
                IdentityUserId = userId
            };

            context.Budgets.Add(budget2);
            context.SaveChanges();
            data.BudgetIds.Add(budget2.Id);

            context.BudgetCategories.AddRange(
                new BudgetCategory { BudgetId = budget2.Id, CategoryId = category1.Id },
                new BudgetCategory { BudgetId = budget2.Id, CategoryId = category2.Id }
            );
            context.BudgetAccounts.AddRange(
                new BudgetAccount { BudgetId = budget2.Id, AccountId = account1.Id },
                new BudgetAccount { BudgetId = budget2.Id, AccountId = account2.Id }
            );
            context.SaveChanges();

            var tx2 = new Transaction
            {
                AccountId = account1.Id,
                CategoryId = category1.Id,
                Amount = 250m,
                Date = DateTime.Now,
                Description = "Transaction 2"
            };
            var tx3 = new Transaction
            {
                AccountId = account2.Id,
                CategoryId = category2.Id,
                Amount = 245m,
                Date = DateTime.Now,
                Description = "Transaction 3"
            };
            context.Transactions.AddRange(tx2, tx3);
            context.SaveChanges();
            data.TransactionIds.AddRange(tx2.Id, tx3.Id );

            return data;
        }

        private BudgetTestData CreateUserWithRecurringTransactions(AppDbContext context, string userId)
        {
            var data = CreateUserWithBudgetsNoTransactions(context, userId);

            var recurringTx = new Transaction
            {
                AccountId = data.AccountId,
                CategoryId = data.CategoryId,
                Amount = 200m,
                Date = DateTime.Now,
                IsReoccuring = true,
                ReoccuranceFrequency = ReoccuranceFrequency.Weekly,
                NextReoccuranceDate = DateTime.Now.AddDays(7),
                Description = "Recurring transaction"
            };

            context.Transactions.Add(recurringTx);
            context.SaveChanges();

            data.RecurringTransactionIds.Add(recurringTx.Id);

            return data;
        }

        private BudgetTestData CreateUserWithOutOfPeriodTransactions(AppDbContext context, string userId)
        {
            var data = CreateUserWithBudgetsNoTransactions(context, userId);

            var lastMonthTx = new Transaction
            {
                AccountId = data.AccountId,
                CategoryId = data.CategoryId,
                Amount = 300m,
                Date = DateTime.Now.AddMonths(-1),
                Description = "Last month transaction (should not be counted)"
            };

            var nextMonthTx = new Transaction
            {
                AccountId = data.AccountId,
                CategoryId = data.CategoryId,
                Amount = 200m,
                Date = DateTime.Now.AddMonths(1),
                Description = "Next month transaction (should not be counted)"
            };

            context.Transactions.AddRange(lastMonthTx, nextMonthTx);
            context.SaveChanges();

            data.OutOfPeriodTransactionIds.AddRange(lastMonthTx.Id, nextMonthTx.Id);

            return data;
        }

        public AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }

    public class BudgetTestData
    {
        public List<int> BudgetIds { get; } = new();
        public List<int> AccountIds { get; } = new();
        public int AccountId { get; set; }
        public List<int> CategoryIds { get; } = new();
        public int CategoryId { get; set; }
        public List<int> TransactionIds { get; } = new();
        public List<int> RecurringTransactionIds { get; } = new();
        public List<int> OutOfPeriodTransactionIds { get; } = new();
    }
}
