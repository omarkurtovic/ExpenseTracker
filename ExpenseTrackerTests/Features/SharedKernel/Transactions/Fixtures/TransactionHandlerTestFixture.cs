using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.SharedKernel.Transactions.Fixtures
{
    public class TransactionHandlerTestFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public string User1Id { get; private set; } = null!;
        public string User2Id { get; private set; } = null!;

        public TransactionTestData User1Data { get; private set; } = null!;
        public TransactionTestData User2Data { get; private set; } = null!;

        public TransactionHandlerTestFixture()
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

            User1Id = Guid.NewGuid().ToString();
            User2Id = Guid.NewGuid().ToString();

            var users = new[]
            {
                CreateUser(User1Id, "user1@test.com"),
                CreateUser(User2Id, "user2@test.com")
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            User1Data = CreateUserWithAccountsAndCategories(context, User1Id);
            User2Data = CreateUserWithAccountsAndCategories(context, User2Id);
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

        private TransactionTestData CreateUserWithAccountsAndCategories(AppDbContext context, string userId)
        {
            var data = new TransactionTestData();

            var checkingAccount = new Account
            {
                Name = "Checking Account",
                InitialBalance = 1000m,
                Icon = "account_balance",
                Color = "#FF6B6B",
                IdentityUserId = userId
            };

            var savingsAccount = new Account
            {
                Name = "Savings Account",
                InitialBalance = 5000m,
                Icon = "savings",
                Color = "#4ECDC4",
                IdentityUserId = userId
            };

            context.Accounts.AddRange(checkingAccount, savingsAccount);
            context.SaveChanges();

            data.AccountIds.Add(checkingAccount.Id);
            data.AccountIds.Add(savingsAccount.Id);

            // Create categories
            var groceryCategory = new Category
            {
                Name = "Groceries",
                Type = TransactionType.Expense,
                IdentityUserId = userId
            };

            var salaryCategory = new Category
            {
                Name = "Salary",
                Type = TransactionType.Income,
                IdentityUserId = userId
            };

            var entertainmentCategory = new Category
            {
                Name = "Entertainment",
                Type = TransactionType.Expense,
                IdentityUserId = userId
            };

            context.Categories.AddRange(groceryCategory, salaryCategory, entertainmentCategory);
            context.SaveChanges();

            data.CategoryIds.Add(groceryCategory.Id);
            data.CategoryIds.Add(salaryCategory.Id);
            data.CategoryIds.Add(entertainmentCategory.Id);

            // Create tags
            var foodTag = new Tag
            {
                Name = "Food",
                Color = "#FF6B6B",
                IdentityUserId = userId
            };

            var necessaryTag = new Tag
            {
                Name = "Necessary",
                Color = "#4ECDC4",
                IdentityUserId = userId
            };

            context.Tags.AddRange(foodTag, necessaryTag);
            context.SaveChanges();

            data.TagIds.Add(foodTag.Id);
            data.TagIds.Add(necessaryTag.Id);

            var sampleTransaction = new Transaction
            {
                AccountId = checkingAccount.Id,
                CategoryId = groceryCategory.Id,
                Amount = -50m,
                Date = DateTime.Now,
                Description = "Grocery shopping"
            };

            context.Transactions.Add(sampleTransaction);
            context.SaveChanges();

            data.TransactionIds.Add(sampleTransaction.Id);

            return data;
        }

        public AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }

    public class TransactionTestData
    {
        public List<int> AccountIds { get; } = new();
        public List<int> CategoryIds { get; } = new();
        public List<int> TagIds { get; } = new();
        public List<int> TransactionIds { get; } = new();
    }
}
