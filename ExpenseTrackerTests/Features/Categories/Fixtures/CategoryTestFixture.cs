using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Categories.Fixtures
{
    public class CategoryTestFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public string NoDataUserId { get; private set; } = null!;
        public string NoCategoriesUserId { get; private set; } = null!;
        public string OneCategory_NoTransactionsUserId { get; private set; } = null!;
        public string OneCategory_WithTransactionsUserId { get; private set; } = null!;
        public string MultipleCategories_WithTransactionsUserId { get; private set; } = null!;

        public CategoryTestData NoData { get; private set; } = null!;
        public CategoryTestData NoCategories { get; private set; } = null!;
        public CategoryTestData OneCategory_NoTransactions { get; private set; } = null!;
        public CategoryTestData OneCategory_WithTransactions { get; private set; } = null!;
        public CategoryTestData MultipleCategories_WithTransactions { get; private set; } = null!;

        public CategoryTestFixture()
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
            NoCategoriesUserId = Guid.NewGuid().ToString();
            OneCategory_NoTransactionsUserId = Guid.NewGuid().ToString();
            OneCategory_WithTransactionsUserId = Guid.NewGuid().ToString();
            MultipleCategories_WithTransactionsUserId = Guid.NewGuid().ToString();

            var users = new[]
            {
                CreateUser(NoDataUserId, "nodata@test.com"),
                CreateUser(NoCategoriesUserId, "nocategories@test.com"),
                CreateUser(OneCategory_NoTransactionsUserId, "onecat_notx@test.com"),
                CreateUser(OneCategory_WithTransactionsUserId, "onecat_tx@test.com"),
                CreateUser(MultipleCategories_WithTransactionsUserId, "multicat_tx@test.com")
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            NoData = new CategoryTestData();
            NoCategories = CreateUserWithNoCategories(context, NoCategoriesUserId);
            OneCategory_NoTransactions = CreateUserWithOneCategoryNoTransactions(context, OneCategory_NoTransactionsUserId);
            OneCategory_WithTransactions = CreateUserWithOneCategoryWithTransactions(context, OneCategory_WithTransactionsUserId);
            MultipleCategories_WithTransactions = CreateUserWithMultipleCategoriesAndTransactions(context, MultipleCategories_WithTransactionsUserId);
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

        private CategoryTestData CreateUserWithNoCategories(AppDbContext context, string userId)
        {
            var data = new CategoryTestData();
            var account = new Account { Name = "Test Account", InitialBalance = 1000m, IdentityUserId = userId };
            
            context.Accounts.Add(account);
            context.SaveChanges();

            data.AccountId = account.Id;
            return data;
        }

        private CategoryTestData CreateUserWithOneCategoryNoTransactions(AppDbContext context, string userId)
        {
            var data = new CategoryTestData();
            
            var account = new Account { Name = "Test Account", InitialBalance = 1000m, IdentityUserId = userId };
            var category = new Category { Name = "Shopping", Type = TransactionType.Expense, IdentityUserId = userId };
            
            context.Accounts.Add(account);
            context.Categories.Add(category);
            context.SaveChanges();

            data.AccountId = account.Id;
            data.CategoryIds.Add(category.Id);

            return data;
        }

        private CategoryTestData CreateUserWithOneCategoryWithTransactions(AppDbContext context, string userId)
        {
            var data = CreateUserWithOneCategoryNoTransactions(context, userId);
            var categoryId = data.CategoryIds.First();
            var accountId = data.AccountId;

            // Add current month transactions
            var currentMonth = DateTime.Now;
            var currentMonthTx = new Transaction
            {
                AccountId = accountId,
                CategoryId = categoryId,
                Amount = -100m,
                Date = currentMonth,
                Description = "Current month transaction",
                IsReoccuring = false
            };

            // Add last month transactions
            var lastMonth = currentMonth.AddMonths(-1);
            var lastMonthTx = new Transaction
            {
                AccountId = accountId,
                CategoryId = categoryId,
                Amount = -150m,
                Date = lastMonth,
                Description = "Last month transaction",
                IsReoccuring = false
            };

            context.Transactions.AddRange(currentMonthTx, lastMonthTx);
            context.SaveChanges();

            data.TransactionIds.Add(currentMonthTx.Id);
            data.TransactionIds.Add(lastMonthTx.Id);

            return data;
        }

        private CategoryTestData CreateUserWithMultipleCategoriesAndTransactions(AppDbContext context, string userId)
        {
            var data = new CategoryTestData();
            
            var account = new Account { Name = "Test Account", InitialBalance = 5000m, IdentityUserId = userId };
            var shoppingCategory = new Category { Name = "Shopping", Type = TransactionType.Expense, IdentityUserId = userId };
            var foodCategory = new Category { Name = "Food", Type = TransactionType.Expense, IdentityUserId = userId };
            var salaryCategory = new Category { Name = "Salary", Type = TransactionType.Income, IdentityUserId = userId };
            
            context.Accounts.Add(account);
            context.Categories.AddRange(shoppingCategory, foodCategory, salaryCategory);
            context.SaveChanges();

            data.AccountId = account.Id;
            data.CategoryIds.AddRange(shoppingCategory.Id, foodCategory.Id, salaryCategory.Id);

            // Add transactions for each category
            var currentMonth = DateTime.Now;
            var lastMonth = currentMonth.AddMonths(-1);

            // Shopping: 200 this month, 100 last month
            var shoppingCurrentTx = new Transaction
            {
                AccountId = account.Id,
                CategoryId = shoppingCategory.Id,
                Amount = -200m,
                Date = currentMonth,
                Description = "Shopping current",
                IsReoccuring = false
            };
            var shoppingLastTx = new Transaction
            {
                AccountId = account.Id,
                CategoryId = shoppingCategory.Id,
                Amount = -100m,
                Date = lastMonth,
                Description = "Shopping last",
                IsReoccuring = false
            };

            // Food: 150 this month, 200 last month
            var foodCurrentTx = new Transaction
            {
                AccountId = account.Id,
                CategoryId = foodCategory.Id,
                Amount = -150m,
                Date = currentMonth,
                Description = "Food current",
                IsReoccuring = false
            };
            var foodLastTx = new Transaction
            {
                AccountId = account.Id,
                CategoryId = foodCategory.Id,
                Amount = -200m,
                Date = lastMonth,
                Description = "Food last",
                IsReoccuring = false
            };

            // Salary: 3000 this month, 3000 last month
            var salaryCurrentTx = new Transaction
            {
                AccountId = account.Id,
                CategoryId = salaryCategory.Id,
                Amount = 3000m,
                Date = currentMonth,
                Description = "Salary current",
                IsReoccuring = false
            };
            var salaryLastTx = new Transaction
            {
                AccountId = account.Id,
                CategoryId = salaryCategory.Id,
                Amount = 3000m,
                Date = lastMonth,
                Description = "Salary last",
                IsReoccuring = false
            };

            context.Transactions.AddRange(
                shoppingCurrentTx, shoppingLastTx,
                foodCurrentTx, foodLastTx,
                salaryCurrentTx, salaryLastTx
            );
            context.SaveChanges();

            data.TransactionIds.AddRange(
                shoppingCurrentTx.Id, shoppingLastTx.Id,
                foodCurrentTx.Id, foodLastTx.Id,
                salaryCurrentTx.Id, salaryLastTx.Id
            );

            return data;
        }

        public AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }

    public class CategoryTestData
    {
        public List<int> CategoryIds { get; } = new();
        public int AccountId { get; set; }
        public List<int> TransactionIds { get; } = new();
    }
}
