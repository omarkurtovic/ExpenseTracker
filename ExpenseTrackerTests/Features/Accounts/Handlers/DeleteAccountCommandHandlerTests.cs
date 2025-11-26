using ExpenseTrackerWebApp.Features.Accounts.Commands;
using ExpenseTrackerWebApp.Features.Accounts.Handlers;
using ExpenseTrackerTests.Features.Accounts.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Accounts.Handlers
{
    public class DeleteAccountCommandHandlerTests : IClassFixture<AccountTestFixture>
    {
        private readonly AccountTestFixture _fixture;

        public DeleteAccountCommandHandlerTests(AccountTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handle_ValidCommand_DeletesAccount()
        {
            using var context = _fixture.CreateContext();
            var accountId = _fixture.AccountNoTransactions.AccountIds[0];
            
            var command = new DeleteAccountCommand()
            {
                Id = accountId,
                UserId = _fixture.AccountNoTransactionsUserId
            };
            var handler = new DeleteAccountCommandHandler(context);

            await handler.Handle(command, CancellationToken.None);

            var deletedAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            Assert.Null(deletedAccount);
        }

        [Fact]
        public async Task Handle_DeleteAccountCascadesToTransactions_DeletesAllRelatedTransactions()
        {
            using var context = _fixture.CreateContext();
            var accountId = _fixture.AccountWithTransactions.AccountIds[0];
            var transactionCountBefore = _fixture.AccountWithTransactions.TransactionIds.Count;
            
            var command = new DeleteAccountCommand()
            {
                Id = accountId,
                UserId = _fixture.AccountWithTransactionsUserId
            };
            var handler = new DeleteAccountCommandHandler(context);

            await handler.Handle(command, CancellationToken.None);

            var remainingTransactions = await context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync();

            Assert.Empty(remainingTransactions);
        }

        [Fact]
        public async Task Handle_AccountNotFound_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            
            var command = new DeleteAccountCommand()
            {
                Id = 999,
                UserId = _fixture.AccountNoTransactionsUserId
            };
            var handler = new DeleteAccountCommandHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var accountId = _fixture.AccountNoTransactions.AccountIds[0];
            
            var command = new DeleteAccountCommand()
            {
                Id = accountId,
                UserId = _fixture.MultipleAccountsUserId
            };
            var handler = new DeleteAccountCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
