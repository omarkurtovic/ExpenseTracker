using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ExpenseTrackerTests.Features.Transactions.Fixtures
{
    public class TransactionTestFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public string UserWithAccountsCategoriesId { get; private set; } = null!;
        public Mock<ISender> UserWithAccountsCategoriesMediatorMock { get; } = new Mock<ISender>();

        public TransactionTestFixture()
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

            UserWithAccountsCategoriesId = Guid.NewGuid().ToString();

            var user = new IdentityUser
            {
                Id = UserWithAccountsCategoriesId,
                UserName = "testuser@test.com",
                NormalizedUserName = "TESTUSER@TEST.COM",
                Email = "testuser@test.com",
                NormalizedEmail = "TESTUSER@TEST.COM",
                EmailConfirmed = true
            };

            context.Users.Add(user);
            context.SaveChanges();
            UserWithAccountsCategoriesId = user.Id;

            var accounts = new List<Account>
            {
                new Account { Name = "Checking", InitialBalance = 1000m, IdentityUserId = user.Id, Icon = "account_balance", Color = "#2196F3" },
                new Account { Name = "Savings", InitialBalance = 5000m, IdentityUserId = user.Id, Icon = "savings", Color = "#4CAF50" }
            };

            var categories = new List<Category>
            {
                new Category { Name = "Groceries", Type = TransactionType.Expense, IdentityUserId = user.Id, Icon = "shopping_cart", Color = "#FF9800" },
                new Category { Name = "Entertainment", Type = TransactionType.Expense, IdentityUserId = user.Id, Icon = "movie", Color = "#9C27B0" },
                new Category { Name = "Salary", Type = TransactionType.Income, IdentityUserId = user.Id, Icon = "attach_money", Color = "#4CAF50" }
            };

            context.Accounts.AddRange(accounts);
            context.Categories.AddRange(categories);
            context.SaveChanges();

            UserWithAccountsCategoriesMediatorMock
                .Setup(m => m.Send(It.IsAny<GetAccountsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);
            UserWithAccountsCategoriesMediatorMock
                .Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);
           
           UserWithAccountsCategoriesMediatorMock
                .Setup(m => m.Send(It.IsAny<ResetToDefaultAccountsCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            UserWithAccountsCategoriesMediatorMock
                .Setup(m => m.Send(It.IsAny<ResetToDefaultCategoriesCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        }

        public AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }
}
