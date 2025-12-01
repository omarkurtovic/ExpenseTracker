using ExpenseTrackerWebApp.Features.Budgets.Queries;
using ExpenseTrackerWebApp.Features.Budgets.Handlers;
using ExpenseTrackerTests.Features.Budgets.Handlers.Fixtures;
using MudBlazor;

namespace ExpenseTrackerTests.Features.Budgets.Handlers
{
    public class GetBudgetsWithProgressQueryHandlerTests : IClassFixture<GetBudgetsWithProgressTestFixture>
    {
        private readonly GetBudgetsWithProgressTestFixture _fixture;

        public GetBudgetsWithProgressQueryHandlerTests(GetBudgetsWithProgressTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handle_UserWithNoData_ReturnsEmptyList()
        {
            using var context = _fixture.CreateContext();
            var query = new GetBudgetsWithProgressQuery { UserId = _fixture.NoDataUserId };
            var handler = new GetBudgetsWithProgressQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);

            Assert.Empty(budgets);
        }

        [Fact]
        public async Task Handle_UserWithBudgetsButNoTransactions_ReturnsZeroProgress()
        {
            using var context = _fixture.CreateContext();
            var query = new GetBudgetsWithProgressQuery { UserId = _fixture.BudgetsNoTransactionsUserId };
            var handler = new GetBudgetsWithProgressQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);

            Assert.Single(budgets);
            var budget = budgets[0];
            Assert.Equal(0m, budget.Spent);
            Assert.Equal(0, budget.Progress);
            Assert.Equal(Color.Success, budget.ProgressColor);
        }

        [Fact]
        public async Task Handle_UserWithSingleBudgetAndTransactions_CalculatesProgressCorrectly()
        {
            using var context = _fixture.CreateContext();
            var query = new GetBudgetsWithProgressQuery { UserId = _fixture.BudgetsWithTransactionsUserId };
            var handler = new GetBudgetsWithProgressQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);

            Assert.Single(budgets);
            var budget = budgets[0];
            Assert.Equal(100m, budget.Spent);
            Assert.Equal(20, budget.Progress); 
            Assert.Equal(Color.Success, budget.ProgressColor);
        }

        [Fact]
        public async Task Handle_BudgetsWithMultipleTransactions_CalculatesProgressForEachBudget()
        {
            using var context = _fixture.CreateContext();
            var query = new GetBudgetsWithProgressQuery { UserId = _fixture.MultipleBudgetsUserId };
            var handler = new GetBudgetsWithProgressQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);

            Assert.NotEmpty(budgets);
            Assert.Equal(2, budgets.Count);
            budgets = budgets.OrderByDescending(b => b.Progress).ToList();
            
            var budget = budgets[0];
            Assert.Equal(740m, budget.Spent);
            Assert.Equal(100, budget.Progress); 
            Assert.Equal(Color.Error, budget.ProgressColor);

            budget = budgets[1];
            Assert.Equal(495m, budget.Spent);
            Assert.Equal(99, budget.Progress); 
            Assert.Equal(Color.Error, budget.ProgressColor);
        }


        [Fact]
        public async Task Handle_WithRecurringTransactions_DoesNotIncludeInProgress()
        {
            using var context = _fixture.CreateContext();
            var query = new GetBudgetsWithProgressQuery { UserId = _fixture.RecurringTransactionUserId };
            var handler = new GetBudgetsWithProgressQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);

            Assert.Single(budgets);
            var budget = budgets[0];
            Assert.Equal(0m, budget.Spent);
            Assert.Equal(0, budget.Progress);
            Assert.Equal(Color.Success, budget.ProgressColor);
        }

        [Fact]
        public async Task Handle_WithOutOfPeriodTransactions_DoesNotIncludeInProgress()
        {
            using var context = _fixture.CreateContext();
            var query = new GetBudgetsWithProgressQuery { UserId = _fixture.OutOfPeriodTransactionUserId };
            var handler = new GetBudgetsWithProgressQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);

            Assert.Single(budgets);
            var budget = budgets[0];
            
            Assert.Equal(0m, budget.Spent);
            Assert.Equal(0, budget.Progress);
            Assert.Equal(Color.Success, budget.ProgressColor);
        }
    }
}
