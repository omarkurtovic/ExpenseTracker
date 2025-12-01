using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Dtos;
using ExpenseTrackerWebApp.SharedKernel.Transactions.Handlers;
using ExpenseTrackerTests.Features.SharedKernel.Transactions.Fixtures;

namespace ExpenseTrackerTests.Features.SharedKernel.Transactions.Handlers
{
    public class CreateTransactionCommandHandlerTests : IDisposable
    {
        private readonly TransactionHandlerTestFixture _fixture;

        public CreateTransactionCommandHandlerTests()
        {
            _fixture = new TransactionHandlerTestFixture();
        }

        [Fact]
        public async Task Handle_ValidCreateTransaction_CreatesTransactionWithAllFields()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 75.50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(14),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Test transaction",
                    IsReoccuring = false,
                    TagIds = new List<int> { _fixture.User1Data.TagIds[0] },
                    TransactionType = TransactionType.Expense
                }
            };

            var transactionId = await handler.Handle(command, CancellationToken.None);

            Assert.True(transactionId > 0);

            var createdTransaction = context.Transactions.FirstOrDefault(t => t.Id == transactionId);
            Assert.NotNull(createdTransaction);
            Assert.Equal(-75.50m, createdTransaction.Amount);
            Assert.Equal(_fixture.User1Data.AccountIds[0], createdTransaction.AccountId);
            Assert.Equal(_fixture.User1Data.CategoryIds[0], createdTransaction.CategoryId);
            Assert.Equal("Test transaction", createdTransaction.Description);
            Assert.False(createdTransaction.IsReoccuring);
            Assert.Equal(DateTime.Today.Add(TimeSpan.FromHours(14)), createdTransaction.Date);
            
            var transactionTags = context.TransactionTags.Where(tt => tt.TransactionId == transactionId).ToList();
            Assert.Single(transactionTags);
            Assert.Equal(_fixture.User1Data.TagIds[0], transactionTags[0].TagId);
        }

        [Fact]
        public async Task Handle_IncomeTransaction_StoresPositiveAmount()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 2000m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(9),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[1], // Salary category
                    Description = "Monthly salary",
                    IsReoccuring = false,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Income
                }
            };

            var transactionId = await handler.Handle(command, CancellationToken.None);
            var createdTransaction = context.Transactions.FirstOrDefault(t => t.Id == transactionId);

            Assert.NotNull(createdTransaction);
            Assert.Equal(2000m, createdTransaction.Amount);
        }

        [Fact]
        public async Task Handle_TransactionWithMultipleTags_CreatesAllTagAssociations()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(10),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Grocery shopping",
                    IsReoccuring = false,
                    TagIds = _fixture.User1Data.TagIds,
                    TransactionType = TransactionType.Expense
                }
            };

            var transactionId = await handler.Handle(command, CancellationToken.None);

            var transactionTags = context.TransactionTags.Where(tt => tt.TransactionId == transactionId).ToList();
            Assert.Equal(2, transactionTags.Count);
        }

        [Fact]
        public async Task Handle_ReoccuringMonthlyTransaction_SetsCorrectNextReoccuranceDate()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var today = DateTime.Today;
            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 100m,
                    Date = today,
                    Time = TimeSpan.FromHours(12),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Monthly subscription",
                    IsReoccuring = true,
                    ReoccuranceFrequency = ReoccuranceFrequency.Monthly,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            var transactionId = await handler.Handle(command, CancellationToken.None);
            var createdTransaction = context.Transactions.FirstOrDefault(t => t.Id == transactionId);

            Assert.NotNull(createdTransaction);
            Assert.True(createdTransaction.IsReoccuring);
            Assert.Equal(ReoccuranceFrequency.Monthly, createdTransaction.ReoccuranceFrequency);
            Assert.Equal(today.AddMonths(1) + TimeSpan.FromHours(12), createdTransaction.NextReoccuranceDate);
        }

        [Fact]
        public async Task Handle_UserCreatingTransactionWithAnotherUserCategory_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(14),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User2Data.CategoryIds[0], 
                    Description = "Unauthorized transaction",
                    IsReoccuring = false,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserCreatingTransactionWithAnotherUserAccount_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(14),
                    AccountId = _fixture.User2Data.AccountIds[0], 
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Unauthorized transaction",
                    IsReoccuring = false,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserCreatingTransactionWithAnotherUserTag_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new CreateTransactionCommandHandler(context);

            var command = new CreateTransactionCommand
            {
                UserId = _fixture.User1Id,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(14),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Unauthorized transaction",
                    IsReoccuring = false,
                    TagIds = new List<int> { _fixture.User2Data.TagIds[0] }, // User2's tag
                    TransactionType = TransactionType.Expense
                }
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
