using ExpenseTrackerWebApp.Features.Accounts.Queries;
using ExpenseTrackerWebApp.Features.Accounts.Handlers;
using ExpenseTrackerTests.Features.Accounts.Fixtures;

namespace ExpenseTrackerTests.Features.Accounts.Handlers
{
    public class GetAccountQueryHandlerTests : IClassFixture<AccountTestFixture>
    {
        private readonly AccountTestFixture _fixture;

        public GetAccountQueryHandlerTests(AccountTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsAccount()
        {
            using var context = _fixture.CreateContext();
            var accountId = _fixture.AccountNoTransactions.AccountIds[0];
            
            var query = new GetAccountQuery()
            {
                Id = accountId,
                UserId = _fixture.AccountNoTransactionsUserId
            };
            var handler = new GetAccountQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_AccountNotFound_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetAccountQuery()
            {
                Id = 999,
                UserId = _fixture.AccountNoTransactionsUserId
            };
            var handler = new GetAccountQueryHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(query, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_UserTriesToAccessOtherUserAccount_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var accountId = _fixture.AccountNoTransactions.AccountIds[0];
            
            var query = new GetAccountQuery()
            {
                Id = accountId,
                UserId = _fixture.MultipleAccountsUserId
            };
            var handler = new GetAccountQueryHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(query, CancellationToken.None)
            );
        }
    }
}
