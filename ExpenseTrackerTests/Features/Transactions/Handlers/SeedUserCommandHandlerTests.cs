using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Transactions.Commands;
using ExpenseTrackerWebApp.Features.Transactions.Dtos;
using ExpenseTrackerWebApp.Features.Transactions.Handlers;
using ExpenseTrackerTests.Features.Transactions.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Transactions.Handlers
{
    public class SeedUserCommandHandlerTests : IDisposable
    {
        private readonly TransactionTestFixture _fixture;

        public SeedUserCommandHandlerTests()
        {
            _fixture = new TransactionTestFixture();
        }

        public void Dispose()
        {
            _fixture?.Dispose();
        }


        [Fact]
        public async Task Handle_ValidCommand_CreatesCorrectNumberOfTransactions()
        {
            using var context = _fixture.CreateContext();
            var mediatorStub = _fixture.UserWithAccountsCategoriesMediatorMock;

            var command = new SeedUserCommand
            {
                UserId = _fixture.UserWithAccountsCategoriesId,
                Options = new SeedDataOptionsDto
                {
                    NumberOfTransaction = 50,
                    TransactionMinAmount = 10,
                    TransactionMaxAmount = 200,
                    TransactionStartDate = DateTime.Today.AddMonths(-3),
                    TransactionEndDate = DateTime.Today,
                    MaxNumberOfTags = 3
                }
            };

            var handler = new SeedUserCommandHandler(context, mediatorStub.Object);
            await handler.Handle(command, CancellationToken.None);

            var transactionCount = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .CountAsync();

            Assert.Equal(50, transactionCount);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesTagsForTransactions()
        {
            using var context = _fixture.CreateContext();
            var mediatorStub = _fixture.UserWithAccountsCategoriesMediatorMock;

            var command = new SeedUserCommand
            {
                UserId = _fixture.UserWithAccountsCategoriesId,
                Options = new SeedDataOptionsDto
                {
                    NumberOfTransaction = 100,
                    TransactionMinAmount = 20,
                    TransactionMaxAmount = 300,
                    TransactionStartDate = DateTime.Today.AddMonths(-6),
                    TransactionEndDate = DateTime.Today,
                    MaxNumberOfTags = 5
                }
            };

            var handler = new SeedUserCommandHandler(context, mediatorStub.Object);
            await handler.Handle(command, CancellationToken.None);

            var tagsCount = await context.Tags
                .Where(t => t.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .CountAsync();

            Assert.Equal(10, tagsCount);

            var transactionsWithTags = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .Where(t => t.TransactionTags.Any())
                .CountAsync();

            Assert.True(transactionsWithTags > 0);
        }

        [Fact]
        public async Task Handle_ValidCommand_DistributesTransactionsAcrossAccounts()
        {
            using var context = _fixture.CreateContext();
            var mediatorStub = _fixture.UserWithAccountsCategoriesMediatorMock;

            var command = new SeedUserCommand
            {
                UserId = _fixture.UserWithAccountsCategoriesId,
                Options = new SeedDataOptionsDto
                {
                    NumberOfTransaction = 100,
                    TransactionMinAmount = 10,
                    TransactionMaxAmount = 200,
                    TransactionStartDate = DateTime.Today.AddMonths(-3),
                    TransactionEndDate = DateTime.Today,
                    MaxNumberOfTags = 2
                }
            };

            var handler = new SeedUserCommandHandler(context, mediatorStub.Object);
            await handler.Handle(command, CancellationToken.None);

            var accountDistribution = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .GroupBy(t => t.AccountId)
                .Select(g => new { AccountId = g.Key, Count = g.Count() })
                .ToListAsync();

            Assert.Equal(2, accountDistribution.Count); 
            Assert.All(accountDistribution, a => Assert.True(a.Count > 0));
        }

        [Fact]
        public async Task Handle_ValidCommand_DistributesTransactionsAcrossCategories()
        {
            using var context = _fixture.CreateContext();
            var mediatorStub = _fixture.UserWithAccountsCategoriesMediatorMock;

            var command = new SeedUserCommand
            {
                UserId = _fixture.UserWithAccountsCategoriesId,
                Options = new SeedDataOptionsDto
                {
                    NumberOfTransaction = 100,
                    TransactionMinAmount = 10,
                    TransactionMaxAmount = 200,
                    TransactionStartDate = DateTime.Today.AddMonths(-3),
                    TransactionEndDate = DateTime.Today,
                    MaxNumberOfTags = 2
                }
            };

            var handler = new SeedUserCommandHandler(context, mediatorStub.Object);
            await handler.Handle(command, CancellationToken.None);

            var categoryDistribution = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .GroupBy(t => t.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            Assert.True(categoryDistribution.Count > 0);
        }

        [Fact]
        public async Task Handle_ValidCommand_SetsCorrectAmountSigns()
        {
            using var context = _fixture.CreateContext();
            var mediatorStub = _fixture.UserWithAccountsCategoriesMediatorMock;

            var command = new SeedUserCommand
            {
                UserId = _fixture.UserWithAccountsCategoriesId,
                Options = new SeedDataOptionsDto
                {
                    NumberOfTransaction = 100,
                    TransactionMinAmount = 10,
                    TransactionMaxAmount = 200,
                    TransactionStartDate = DateTime.Today.AddMonths(-3),
                    TransactionEndDate = DateTime.Today,
                    MaxNumberOfTags = 2
                }
            };

            var handler = new SeedUserCommandHandler(context, mediatorStub.Object);
            await handler.Handle(command, CancellationToken.None);

            var expenseTransactions = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .Where(t => t.Category.Type == TransactionType.Expense)
                .ToListAsync();

            var incomeTransactions = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .Where(t => t.Category.Type == TransactionType.Income)
                .ToListAsync();

            Assert.All(expenseTransactions, t => Assert.True(t.Amount < 0, $"Expense transaction should be negative, got {t.Amount}"));

            Assert.All(incomeTransactions, t => Assert.True(t.Amount > 0, $"Income transaction should be positive, got {t.Amount}"));
        }

        [Fact]
        public async Task Handle_ValidCommand_TransactionsWithinDateRange()
        {
            using var context = _fixture.CreateContext();
            var mediatorStub = _fixture.UserWithAccountsCategoriesMediatorMock;

            var startDate = DateTime.Today.AddMonths(-2);
            var endDate = DateTime.Today;

            var command = new SeedUserCommand
            {
                UserId = _fixture.UserWithAccountsCategoriesId,
                Options = new SeedDataOptionsDto
                {
                    NumberOfTransaction = 100,
                    TransactionMinAmount = 10,
                    TransactionMaxAmount = 200,
                    TransactionStartDate = startDate,
                    TransactionEndDate = endDate,
                    MaxNumberOfTags = 2
                }
            };

            var handler = new SeedUserCommandHandler(context, mediatorStub.Object);
            await handler.Handle(command, CancellationToken.None);

            var transactions = await context.Transactions
                .Where(t => t.Account.IdentityUserId == _fixture.UserWithAccountsCategoriesId)
                .ToListAsync();

            Assert.All(transactions, t =>
            {
                Assert.True(t.Date.Date >= startDate, $"Transaction date {t.Date} should be >= {startDate}");
                Assert.True(t.Date.Date <= endDate, $"Transaction date {t.Date} should be <= {endDate}");
            });
        }
    }
}
