using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Handlers;
using ExpenseTrackerTests.Features.SharedKernel.Transactions.Fixtures;

namespace ExpenseTrackerTests.Features.SharedKernel.Transactions.Handlers
{
    public class DeleteTransactionCommandHandlerTests : IDisposable
    {
        private readonly TransactionHandlerTestFixture _fixture;

        public DeleteTransactionCommandHandlerTests()
        {
            _fixture = new TransactionHandlerTestFixture();
        }

        [Fact]
        public async Task Handle_ValidDeleteTransaction_RemovesTransaction()
        {
            var context = _fixture.CreateContext();
            var handler = new DeleteTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new DeleteTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = transactionId
            };

            await handler.Handle(command, CancellationToken.None);

            var deletedTransaction = context.Transactions.FirstOrDefault(t => t.Id == transactionId);
            Assert.Null(deletedTransaction);
        }

        [Fact]
        public async Task Handle_UserDeletingAnotherUserTransaction_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new DeleteTransactionCommandHandler(context);

            var transactionId = _fixture.User1Data.TransactionIds[0];

            var command = new DeleteTransactionCommand
            {
                UserId = _fixture.User2Id,
                Id = transactionId
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeletingNonexistentTransaction_ThrowsUnauthorizedAccessException()
        {
            var context = _fixture.CreateContext();
            var handler = new DeleteTransactionCommandHandler(context);

            var command = new DeleteTransactionCommand
            {
                UserId = _fixture.User1Id,
                Id = 99999
            };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        public void Dispose() => _fixture.Dispose();
    }
}
