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
using ExpenseTrackerTests.Features.Budgets.Handlers.Fixtures;

namespace ExpenseTrackerTests.Features.Budgets.Handlers
{
    public class DeleteBudgetCommandHandlerTests : IClassFixture<GetBudgetsWithProgressTestFixture>
    {
        private readonly GetBudgetsWithProgressTestFixture _fixture;

        public DeleteBudgetCommandHandlerTests(GetBudgetsWithProgressTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Handle_BudgetDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            var command = new DeleteBudgetCommand();
            command.UserId = _fixture.NoBudgetsUserId;
            command.Id = 999;

            var handler = new DeleteBudgetCommandHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }


        [Fact]
        public async Task Handle_BudgetDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            var command = new DeleteBudgetCommand();
            command.UserId = _fixture.NoBudgetsUserId;
            command.Id = _fixture.BudgetsNoTransactions.BudgetIds.First();

            var handler = new DeleteBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_ValidBudget_DeletsSuccesfully()
        {
            using var context = _fixture.CreateContext();

            var command = new DeleteBudgetCommand();
            command.UserId = _fixture.BudgetsNoTransactionsUserId;
            command.Id = _fixture.BudgetsNoTransactions.BudgetIds.First();

            var handler = new DeleteBudgetCommandHandler(context);
            await handler.Handle(command, CancellationToken.None);

            var bugets = await context.Budgets.Where(b => b.IdentityUserId == _fixture.BudgetsNoTransactionsUserId).ToListAsync();
            var bugetsAccounts = await context.BudgetAccounts.Include(ba => ba.Budget)
                .Where(ba => ba.Budget.IdentityUserId == _fixture.BudgetsNoTransactionsUserId)
                .ToListAsync();
            var budgetCategories = await context.BudgetCategories.Include(bc => bc.Budget)
                .Where(bc => bc.Budget.IdentityUserId == _fixture.BudgetsNoTransactionsUserId)
                .ToListAsync();

            Assert.Empty(bugets);
            Assert.Empty(bugetsAccounts);
            Assert.Empty(budgetCategories);
        }
    }
}