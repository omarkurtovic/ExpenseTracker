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
    // Placeholder for CreateBudgetCommandHandlerTests
    public class CreateBudgetCommandHandlerTests : IClassFixture<GetBudgetsWithProgressTestFixture>
    {
        private readonly GetBudgetsWithProgressTestFixture _fixture;

        public CreateBudgetCommandHandlerTests(GetBudgetsWithProgressTestFixture fixture)
        {
            _fixture = fixture;
        }
        private CreateBudgetCommand CreateValidCreateBudgetCommand()
        {
            var budgetDto = new BudgetDto();
            budgetDto.Name = "Monthly Budget";
            budgetDto.BudgetType = BudgetType.Monthly;
            budgetDto.Amount = 1000m;
            budgetDto.Description = "This is a test budget.";

            var result = new CreateBudgetCommand();
            result.BudgetDto = budgetDto;
            return result;
        }

        [Fact]
        public async Task Handle_CategoryDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.IdentityUserId = _fixture.NoBudgetsUserId;
            command.BudgetDto.Accounts = [new Account(){Id = _fixture.NoBudgets.AccountId}];
            command.BudgetDto.Categories = [new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}];

            var handler = new CreateBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.IdentityUserId = _fixture.NoBudgetsUserId;
            command.BudgetDto.Accounts = [new Account(){Id = _fixture.NoBudgets.AccountId}];
            command.BudgetDto.Categories = [new Category(){Id = 999}];

            var handler = new CreateBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.IdentityUserId = _fixture.NoBudgetsUserId;
            command.BudgetDto.Accounts = [new Account(){Id = _fixture.BudgetsNoTransactions.AccountId}];
            command.BudgetDto.Categories = [new Category(){Id = _fixture.NoBudgets.CategoryId}];

            var handler = new CreateBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.IdentityUserId = _fixture.NoBudgetsUserId;
            command.BudgetDto.Accounts = [new Account(){Id = 999}];
            command.BudgetDto.Categories = [new Category(){Id = _fixture.NoBudgets.CategoryId}];

            var handler = new CreateBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }



        [Fact]
        public async Task Handle_ValidCommand_CreatesBudget()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.IdentityUserId = _fixture.NoBudgetsUserId;
            command.BudgetDto.Accounts = [new Account(){Id = _fixture.NoBudgets.AccountId}];
            command.BudgetDto.Categories = [new Category(){Id = _fixture.NoBudgets.CategoryId}];

            var handler = new CreateBudgetCommandHandler(context);
            var budgetId = await handler.Handle(command, CancellationToken.None);

            var createdBudget = await context.Budgets
                .Include(b => b.BudgetCategories)
                .Include(b => b.BudgetAccounts)
                .FirstOrDefaultAsync(b => b.Id == budgetId);

            Assert.NotNull(createdBudget);
            Assert.Equal(command.BudgetDto.Name, createdBudget!.Name);
            Assert.Equal(command.BudgetDto.BudgetType, createdBudget.BudgetType);
            Assert.Equal(command.BudgetDto.Amount, createdBudget.Amount);
            Assert.Equal(command.BudgetDto.IdentityUserId, createdBudget.IdentityUserId);
            Assert.Equal(command.BudgetDto.Description, createdBudget.Description);
            Assert.Equal(command.BudgetDto.Categories.Count(), createdBudget.BudgetCategories.Count);
            Assert.Equal(command.BudgetDto.Accounts.Count(), createdBudget.BudgetAccounts.Count);

            Assert.Equal(command.BudgetDto.Categories.First().Id, createdBudget.BudgetCategories.First().CategoryId);
            Assert.Equal(_fixture.NoBudgets.CategoryId, createdBudget.BudgetCategories.First().CategoryId);

            Assert.Equal(command.BudgetDto.Accounts.First().Id, createdBudget.BudgetAccounts.First().AccountId);
            Assert.Equal(_fixture.NoBudgets.AccountId, createdBudget.BudgetAccounts.First().AccountId);
        }


    }
}