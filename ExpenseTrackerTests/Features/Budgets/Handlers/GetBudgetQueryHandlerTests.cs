using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Handlers;
using ExpenseTrackerWebApp.Features.Budgets.Queries;
using ExpenseTrackerTests.Features.Budgets.Handlers.Fixtures;

namespace ExpenseTrackerTests.Features.Budgets.Handlers
{
    public class GetBudgetQueryHandlerTests : IClassFixture<GetBudgetsWithProgressTestFixture>
    {
        private readonly GetBudgetsWithProgressTestFixture _fixture;

        public GetBudgetQueryHandlerTests(GetBudgetsWithProgressTestFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public async Task Handle_BudgetDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetBudgetQuery()
            {
                Id = _fixture.BudgetsNoTransactions.BudgetIds.First(),
                UserId = _fixture.NoBudgetsUserId
            };

            var handler = new GetBudgetQueryHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(query, CancellationToken.None)
            );
        }

        
        [Fact]
        public async Task Handle_BudgetDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetBudgetQuery(){
                Id = 999,
                UserId = _fixture.NoBudgetsUserId
            };

            var handler = new GetBudgetQueryHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(query, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsBudget()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetBudgetQuery()
            {
                Id = _fixture.BudgetsNoTransactions.BudgetIds.First(),
                UserId = _fixture.BudgetsNoTransactionsUserId
            };
            var handler = new GetBudgetQueryHandler(context);

            var result =  await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Test Budget", result.Name);
            Assert.Equal(500, result.Amount);
            Assert.Equal(BudgetType.Monthly, result.BudgetType);
            Assert.Single(result.Accounts);
            Assert.Single(result.Categories);
        }

    }
}