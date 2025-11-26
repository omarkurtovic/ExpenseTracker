using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
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

        private BudgetDto CreateValidBudgetDto()
        {
            var budgetDto = new BudgetDto();
            budgetDto.Name = "Bob's budget - change";
            budgetDto.BudgetType = BudgetType.Yearly;
            budgetDto.Amount = 1000m;
            budgetDto.Description = "This is a test budget.";
            return budgetDto;
        }

        [Fact]
        public async Task Handle_CategoryDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = _fixture.BudgetsNoTransactions.AccountId}];
            budgetDto.Categories = [new Category(){Id = _fixture.NoBudgets.CategoryId}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.BudgetsNoTransactionsUserId, Id = _fixture.BudgetsNoTransactions.BudgetIds.First()};
            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = _fixture.BudgetsNoTransactions.AccountId}];
            budgetDto.Categories = [new Category(){Id = 999}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.BudgetsNoTransactionsUserId, Id = _fixture.BudgetsNoTransactions.BudgetIds.First()};
            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = _fixture.NoBudgets.AccountId}];
            budgetDto.Categories = [new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.BudgetsNoTransactionsUserId, Id = _fixture.BudgetsNoTransactions.BudgetIds.First()};
            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = 999}];
            budgetDto.Categories = [new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.BudgetsNoTransactionsUserId, Id = _fixture.BudgetsNoTransactions.BudgetIds.First()};
            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_BudgetDoesNotExist_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = _fixture.BudgetsNoTransactions.AccountId}];
            budgetDto.Categories = [new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.BudgetsNoTransactionsUserId, Id = 999};
            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_BudgetDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = _fixture.BudgetsNoTransactions.AccountId}];
            budgetDto.Categories = [new Category(){Id = _fixture.BudgetsNoTransactions.CategoryId}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.BudgetsNoTransactionsUserId, Id = _fixture.BudgetsWithTransactions.BudgetIds.First()};
            var handler = new EditBudgetCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_EditsBudget()
        {
            using var context = _fixture.CreateContext();
            var budgetDto = CreateValidBudgetDto();
            budgetDto.Accounts = [new(){Id = _fixture.MultipleBudgets.AccountIds.First()}];
            budgetDto.Categories = [new Category(){Id = _fixture.MultipleBudgets.CategoryIds.First()}];
            
            var command = new EditBudgetCommand(){BudgetDto = budgetDto, 
                UserId = _fixture.MultipleBudgetsUserId, Id = _fixture.MultipleBudgets.BudgetIds.First()};
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