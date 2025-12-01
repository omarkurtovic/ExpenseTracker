using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Dtos;
using ExpenseTrackerWebApp.SharedKernel.Transactions.Handlers;
using ExpenseTrackerTests.Features.SharedKernel.Transactions.Fixtures;

namespace ExpenseTrackerTests.Features.SharedKernel.Transactions.Handlers
{
    public class EditTransactionCommandHandlerTests : IDisposable
    {
        private readonly TransactionHandlerTestFixture _fixture;

        public EditTransactionCommandHandlerTests()
        {
            _fixture = new TransactionHandlerTestFixture();
        }

        [Fact]
        public async Task Handle_ValidEditTransaction_UpdatesAllFields()
        {
            var context = _fixture.CreateContext();
            var handler = new EditTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new EditTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = transactionId,
                TransactionDto = new TransactionDto
                {
                    Amount = 125.75m,
                    Date = DateTime.Today.AddDays(-1),
                    Time = TimeSpan.FromHours(15),
                    AccountId = _fixture.User1Data.AccountIds[1],
                    CategoryId = _fixture.User1Data.CategoryIds[2],
                    Description = "Updated transaction",
                    IsReoccuring = false,
                    TagIds = new List<int> { _fixture.User1Data.TagIds[1] },
                    TransactionType = TransactionType.Expense
                }
            };

            await handler.Handle(command, CancellationToken.None);

            var updatedTransaction = context.Transactions.FirstOrDefault(t => t.Id == transactionId);
            Assert.NotNull(updatedTransaction);
            Assert.Equal(-125.75m, updatedTransaction.Amount);
            Assert.Equal(_fixture.User1Data.AccountIds[1], updatedTransaction.AccountId);
            Assert.Equal(_fixture.User1Data.CategoryIds[2], updatedTransaction.CategoryId);
            Assert.Equal("Updated transaction", updatedTransaction.Description);
            Assert.Equal(DateTime.Today.AddDays(-1).Add(TimeSpan.FromHours(15)), updatedTransaction.Date);
            Assert.False(updatedTransaction.IsReoccuring);
            
            var transactionTags = context.TransactionTags.Where(tt => tt.TransactionId == transactionId).ToList();
            Assert.Single(transactionTags);
            Assert.Equal(_fixture.User1Data.TagIds[1], transactionTags[0].TagId);
        }

        [Fact]
        public async Task Handle_EditTransactionToRecurring_SetsNextReoccuranceDate()
        {
            var context = _fixture.CreateContext();
            var handler = new EditTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];
            var editDate = DateTime.Today;

            var command = new EditTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = transactionId,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = editDate,
                    Time = TimeSpan.FromHours(12),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Now recurring",
                    IsReoccuring = true,
                    ReoccuranceFrequency = ReoccuranceFrequency.Weekly,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            await handler.Handle(command, CancellationToken.None);

            var updatedTransaction = context.Transactions.FirstOrDefault(t => t.Id == transactionId);
            Assert.NotNull(updatedTransaction);
            Assert.True(updatedTransaction.IsReoccuring);
            Assert.Equal(ReoccuranceFrequency.Weekly, updatedTransaction.ReoccuranceFrequency);
            Assert.Equal(editDate.AddDays(7) + TimeSpan.FromHours(12), updatedTransaction.NextReoccuranceDate);
        }

        [Fact]
        public async Task Handle_UserEditingAnotherUserTransaction_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new EditTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new EditTransactionCommand
            {
                UserId = _fixture.User2Id,
                Id = transactionId,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(12),
                    AccountId = _fixture.User2Data.AccountIds[0],
                    CategoryId = _fixture.User2Data.CategoryIds[0],
                    Description = "Unauthorized edit",
                    IsReoccuring = false,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EditTransactionWithAnotherUserCategory_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new EditTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new EditTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = transactionId,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(12),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User2Data.CategoryIds[0], 
                    Description = "Unauthorized category",
                    IsReoccuring = false,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EditTransactionWithAnotherUserAccount_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new EditTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new EditTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = transactionId,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(12),
                    AccountId = _fixture.User2Data.AccountIds[0], 
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Unauthorized account",
                    IsReoccuring = false,
                    TagIds = new List<int>(),
                    TransactionType = TransactionType.Expense
                }
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_EditTransactionWithAnotherUserTag_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new EditTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new EditTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = transactionId,
                TransactionDto = new TransactionDto
                {
                    Amount = 50m,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(12),
                    AccountId = _fixture.User1Data.AccountIds[0],
                    CategoryId = _fixture.User1Data.CategoryIds[0],
                    Description = "Unauthorized tag",
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
