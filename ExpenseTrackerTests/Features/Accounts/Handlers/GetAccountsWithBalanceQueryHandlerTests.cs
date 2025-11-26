using ExpenseTrackerWebApp.Features.Accounts.Queries;
using ExpenseTrackerWebApp.Features.Accounts.Handlers;
using ExpenseTrackerTests.Features.Accounts.Fixtures;

namespace ExpenseTrackerTests.Features.Accounts.Handlers
{
    public class GetAccountsWithBalanceQueryHandlerTests : IClassFixture<AccountTestFixture>
    {
        private readonly AccountTestFixture _fixture;

        public GetAccountsWithBalanceQueryHandlerTests(AccountTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handle_UserWithNoAccounts_ReturnsEmptyList()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetAccountsWithBalanceQuery()
            {
                UserId = _fixture.NoAccountsUserId
            };
            var handler = new GetAccountsWithBalanceQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_UserWithOneAccount_ReturnsOneAccount()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetAccountsWithBalanceQuery()
            {
                UserId = _fixture.AccountNoTransactionsUserId
            };
            var handler = new GetAccountsWithBalanceQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Checking Account", result[0].Name);
        }

        [Fact]
        public async Task Handle_UserWithMultipleAccounts_ReturnsAllAccounts()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetAccountsWithBalanceQuery()
            {
                UserId = _fixture.MultipleAccountsUserId
            };
            var handler = new GetAccountsWithBalanceQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(3, result.Count);
            Assert.All(result, account => Assert.NotNull(account.Name));
        }

        [Fact]
        public async Task Handle_CalculatesCurrentBalanceCorrectly_WithMultipleTransactions()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetAccountsWithBalanceQuery()
            {
                UserId = _fixture.MultipleAccountsUserId
            };
            var handler = new GetAccountsWithBalanceQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            // Account 1 (Checking): InitialBalance 1000m + tx1 (-75) + tx2 (-30) = 895m
            var account1 = result.First(a => a.Name == "Checking Account");
            Assert.Equal(895m, account1.CurrentBalance);

            // Account 2 (Savings): InitialBalance 5000m + tx3 (2000) = 7000m
            var account2 = result.First(a => a.Name == "Savings Account");
            Assert.Equal(7000m, account2.CurrentBalance);

            // Account 3 (Credit Card): InitialBalance 0m + tx4 (-150) = -150m
            var account3 = result.First(a => a.Name == "Credit Card");
            Assert.Equal(-150m, account3.CurrentBalance);
        }

        [Fact]
        public async Task Handle_PreservesAccountProperties()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetAccountsWithBalanceQuery()
            {
                UserId = _fixture.MultipleAccountsUserId
            };
            var handler = new GetAccountsWithBalanceQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            var checkingAccount = result.First(a => a.Name == "Checking Account");
            Assert.Equal("account_balance", checkingAccount.Icon);
            Assert.Equal("#FF6B6B", checkingAccount.Color);
            Assert.Equal(1000m, checkingAccount.InitialBalance);
        }
    }
}
