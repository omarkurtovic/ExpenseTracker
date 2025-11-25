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
    public class EditBudgetCommandHandlerTests : IClassFixture<GetBudgetsWithProgressTestFixture>
    {
        private readonly GetBudgetsWithProgressTestFixture _fixture;

        public EditBudgetCommandHandlerTests(GetBudgetsWithProgressTestFixture fixture)
        {
            _fixture = fixture;
        }

        private EditBudgetCommand CreateValidEditBudgetCommand()
        {
            var budgetDto = new BudgetDto();
            budgetDto.Name = "Bob's budget - change";
            budgetDto.BudgetType = BudgetType.Yearly;
            budgetDto.Amount = 1000m;
            budgetDto.Description = "This is a test budget.";

            var result = new EditBudgetCommand();
            result.BudgetDto = budgetDto;
            return result;
        }

        [Fact]
        public async Task Handle_CategoryDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            var command = CreateValidEditBudgetCommand();
            command.Id = _fixture.BudgetsNoTransactions.BudgetIds.First();
            command.BudgetDto.IdentityUserId = _fixture.BudgetsNoTransactionsUserId;
            command.BudgetDto.Accounts = new List<Account>(){new Account(){Id = _fixture.BudgetsNoTransactions.AccountId}};
            command.BudgetDto.Categories = new List<Category>(){new Category(){Id = _fixture.NoBudgets.CategoryId}};

            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            var command = CreateValidEditBudgetCommand();
            command.Id = _fixture.BudgetsNoTransactions.BudgetIds.First();
            command.BudgetDto.IdentityUserId = _fixture.BudgetsNoTransactionsUserId;
            command.BudgetDto.Accounts = new List<Account>(){new Account(){Id = _fixture.BudgetsNoTransactions.AccountId}};
            command.BudgetDto.Categories = new List<Category>(){new Category(){Id = 999}};

            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            var command = CreateValidEditBudgetCommand();
            command.Id = _fixture.BudgetsNoTransactions.BudgetIds.First();
            command.BudgetDto.IdentityUserId = _fixture.BudgetsNoTransactionsUserId;
            command.BudgetDto.Accounts = new List<Account>(){new Account(){Id = _fixture.NoBudgets.AccountId}};
            command.BudgetDto.Categories = new List<Category>(){new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}};

            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidEditBudgetCommand();
            command.Id = _fixture.BudgetsNoTransactions.BudgetIds.First();
            command.BudgetDto.IdentityUserId = _fixture.BudgetsNoTransactionsUserId;
            command.BudgetDto.Accounts = new List<Account>(){new Account(){Id = 999}};
            command.BudgetDto.Categories = new List<Category>(){new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}};

            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_BudgetDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidEditBudgetCommand();
            command.Id = 999;
            command.BudgetDto.IdentityUserId = _fixture.BudgetsNoTransactionsUserId;
            command.BudgetDto.Accounts = new List<Account>(){new Account(){Id = _fixture.BudgetsNoTransactions.AccountId}};
            command.BudgetDto.Categories = new List<Category>(){new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}};

            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_BudgetDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidEditBudgetCommand();
            command.Id = _fixture.BudgetsWithTransactions.BudgetIds.First();
            command.BudgetDto.IdentityUserId = _fixture.BudgetsNoTransactionsUserId;
            command.BudgetDto.Accounts = new List<Account>(){new Account(){Id = _fixture.BudgetsNoTransactions.AccountId}};
            command.BudgetDto.Categories = new List<Category>(){new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}};

            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_EditsBudget()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidEditBudgetCommand();
            command.Id = _fixture.MultipleBudgets.BudgetIds.First();
            command.BudgetDto.IdentityUserId = _fixture.MultipleBudgetsUserId;
            command.BudgetDto.Accounts = new List<Account>() { new Account() { Id = _fixture.MultipleBudgets.AccountIds.First() } };
            command.BudgetDto.Categories = new List<Category>() { new Category() { Id = _fixture.MultipleBudgets.CategoryIds.First() } };

            var handler = new EditBudgetCommandHandler(context);

            await handler.Handle(command, CancellationToken.None);

            var editedBudget = await context.Budgets
                .Include(b => b.BudgetCategories)
                .Include(b => b.BudgetAccounts)
                .FirstOrDefaultAsync(b => b.Id == command.Id);

            Assert.NotNull(editedBudget);
            Assert.Equal(command.BudgetDto.Name, editedBudget!.Name);
            Assert.Equal(command.BudgetDto.BudgetType, editedBudget.BudgetType);
            Assert.Equal(command.BudgetDto.Amount, editedBudget.Amount);
            Assert.Equal(command.BudgetDto.IdentityUserId, editedBudget.IdentityUserId);
            Assert.Equal(command.BudgetDto.Description, editedBudget.Description);
            Assert.Equal(command.BudgetDto.Categories.Count(), editedBudget.BudgetCategories.Count);
            Assert.Equal(command.BudgetDto.Accounts.Count(), editedBudget.BudgetAccounts.Count);

            Assert.Equal(command.BudgetDto.Categories.First().Id, editedBudget.BudgetCategories.First().CategoryId);
            Assert.Equal(_fixture.MultipleBudgets.CategoryIds.First(), editedBudget.BudgetCategories.First().CategoryId);

            Assert.Equal(command.BudgetDto.Accounts.First().Id, editedBudget.BudgetAccounts.First().AccountId);
            Assert.Equal(_fixture.MultipleBudgets.AccountIds.First(), editedBudget.BudgetAccounts.First().AccountId);
        }


    }
}