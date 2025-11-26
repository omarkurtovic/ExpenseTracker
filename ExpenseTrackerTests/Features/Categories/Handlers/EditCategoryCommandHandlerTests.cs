using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Commands;
using ExpenseTrackerWebApp.Features.Categories.Dtos;
using ExpenseTrackerWebApp.Features.Categories.Handlers;
using ExpenseTrackerTests.Features.Categories.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTrackerTests.Features.Categories.Handlers
{
    public class EditCategoryCommandHandlerTests : IClassFixture<CategoryTestFixture>
    {
        private readonly CategoryTestFixture _fixture;

        public EditCategoryCommandHandlerTests(CategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        private EditCategoryCommand CreateValidEditCategoryCommand(int categoryId, string userId, string newName = "Updated Category")
        {
            var categoryDto = new CategoryDto
            {
                Name = newName,
                Type = TransactionType.Expense,
                Icon = "star",
                Color = "#00FF00"
            };

            var command = new EditCategoryCommand
            {
                Id = categoryId,
                UserId = userId,
                CategoryDto = categoryDto
            };

            return command;
        }

        [Fact]
        public async Task Handle_ValidCommand_UpdatesCategory()
        {
            var testData = _fixture.OneCategory_NoTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();
            var command = CreateValidEditCategoryCommand(categoryId, _fixture.OneCategory_NoTransactionsUserId, "New Name");
            var handler = new EditCategoryCommandHandler(context);

            await handler.Handle(command, CancellationToken.None);

            var updatedCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            Assert.NotNull(updatedCategory);
            Assert.Equal("New Name", updatedCategory!.Name);
            Assert.Equal("star", updatedCategory.Icon);
            Assert.Equal("#00FF00", updatedCategory.Color);
        }

        [Fact]
        public async Task Handle_UpdateDifferentUser_ThrowsUnauthorizedAccessException()
        {
            var testData = _fixture.OneCategory_NoTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();
            
            // Try to edit with a different user's ID
            var command = CreateValidEditCategoryCommand(categoryId, "different-user-id", "Hacked Name");
            var handler = new EditCategoryCommandHandler(context);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_UpdateNonExistentCategory_ThrowsArgumentException()
        {
            using var context = _fixture.CreateContext();
            var nonExistentId = 99999;
            var command = CreateValidEditCategoryCommand(nonExistentId, _fixture.OneCategory_NoTransactionsUserId, "Attempt Update");
            var handler = new EditCategoryCommandHandler(context);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_UpdateMultipleFields_AllUpdateCorrectly()
        {
            var testData = _fixture.OneCategory_NoTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();
            
            var categoryDto = new CategoryDto
            {
                Name = "Completely New",
                Type = TransactionType.Income,
                Icon = "money",
                Color = "#FFD700"
            };

            var command = new EditCategoryCommand
            {
                Id = categoryId,
                UserId = _fixture.OneCategory_NoTransactionsUserId,
                CategoryDto = categoryDto
            };

            var handler = new EditCategoryCommandHandler(context);
            await handler.Handle(command, CancellationToken.None);

            var updated = await context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            Assert.NotNull(updated);
            Assert.Equal("Completely New", updated!.Name);
            Assert.Equal(TransactionType.Income, updated.Type);
            Assert.Equal("money", updated.Icon);
            Assert.Equal("#FFD700", updated.Color);
        }

        [Fact]
        public async Task Handle_ChangeCategoryType_MakesAmountsPositiveForIncome()
        {
            var testData = _fixture.OneCategory_WithTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();

            var originalTransactions = await context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();

            Assert.All(originalTransactions, t => Assert.True(t.Amount < 0, "Expense transactions should be negative"));

            var categoryDto = new CategoryDto
            {
                Name = "Updated Category",
                Type = TransactionType.Income,
                Icon = "money",
                Color = "#00FF00"
            };

            var command = new EditCategoryCommand
            {
                Id = categoryId,
                UserId = _fixture.OneCategory_WithTransactionsUserId,
                CategoryDto = categoryDto
            };

            var handler = new EditCategoryCommandHandler(context);
            await handler.Handle(command, CancellationToken.None);

            var updatedTransactions = await context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();

            Assert.All(updatedTransactions, t => Assert.True(t.Amount > 0));
        }

        [Fact]
        public async Task Handle_ChangeCategoryTypeIncomeToExpense_MakesAmountsNegative()
        {
            using var context = _fixture.CreateContext();
            var userId = _fixture.MultipleCategories_WithTransactionsUserId;

            // Get the Salary category (Income type)
            var salaryCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == "Salary" && c.IdentityUserId == userId);

            Assert.NotNull(salaryCategory);
            Assert.Equal(TransactionType.Income, salaryCategory.Type);

            // Get original transactions (should all be positive for Income)
            var originalTransactions = await context.Transactions
                .Where(t => t.CategoryId == salaryCategory.Id)
                .ToListAsync();

            Assert.All(originalTransactions, t => Assert.True(t.Amount > 0, "Income transactions should be positive"));

            // Change category type from Income to Expense
            var categoryDto = new CategoryDto
            {
                Name = "Salary",
                Type = TransactionType.Expense,
                Icon = "money",
                Color = "#FF0000"
            };

            var command = new EditCategoryCommand
            {
                Id = salaryCategory.Id,
                UserId = userId,
                CategoryDto = categoryDto
            };

            var handler = new EditCategoryCommandHandler(context);
            await handler.Handle(command, CancellationToken.None);

            // Verify transaction amounts are now negative (for Expense)
            var updatedTransactions = await context.Transactions
                .Where(t => t.CategoryId == salaryCategory.Id)
                .ToListAsync();

            Assert.Equal(originalTransactions.Count, updatedTransactions.Count);

            // All should be negative now
            Assert.All(updatedTransactions, t => Assert.True(t.Amount < 0, "Expense transactions should be negative"));
        }
    }
}
