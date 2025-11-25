using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerWebApp.Features.Budgets.Handlers;
using Microsoft.AspNetCore.Identity;
using ExpenseTrackerWebApp.Features.Budgets.Queries;
using ExpenseTrackerTests.Features.Budgets.Handlers.Fixtures;

namespace ExpenseTrackerTests.Features.Budgets.Handlers
{
    public class GetBudgetsQueryHandlerTests : IClassFixture<GetBudgetsWithProgressTestFixture>
    {
        private readonly GetBudgetsWithProgressTestFixture _fixture;

        public GetBudgetsQueryHandlerTests(GetBudgetsWithProgressTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handle_UserHasNoBudgets_ReturnsEmptyList()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetBudgetsQuery();
            query.UserId = _fixture.NoBudgetsUserId;

            var handler = new GetBudgetsQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);
            Assert.Empty(budgets);
        }

        [Fact]
        public async Task Handle_UserHasOneBudget_ReturnsValidList()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetBudgetsQuery();
            query.UserId = _fixture.BudgetsNoTransactionsUserId;

            var handler = new GetBudgetsQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);
            Assert.Single(budgets);
            Assert.Equal(_fixture.BudgetsNoTransactionsUserId, budgets[0].IdentityUserId);
        }

        [Fact]
        public async Task Handle_UserHasMultipleBudgets_ReturnsValidList()
        {
            using var context = _fixture.CreateContext();
            
            var query = new GetBudgetsQuery();
            query.UserId = _fixture.MultipleBudgetsUserId;

            var handler = new GetBudgetsQueryHandler(context);

            var budgets = await handler.Handle(query, CancellationToken.None);
            Assert.Equal(2, budgets.Count);
        }
    }
}