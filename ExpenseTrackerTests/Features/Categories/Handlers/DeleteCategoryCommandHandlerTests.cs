using ExpenseTrackerWebApp.Features.Categories.Commands;
using ExpenseTrackerWebApp.Features.Categories.Handlers;
using ExpenseTrackerTests.Features.Categories.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DeleteCategoryCommandHandler = ExpenseTrackerWebApp.Features.Categories.Handlers.DeleteCategoryCommandHandler;

namespace ExpenseTrackerTests.Features.Categories.Handlers
{
    public class DeleteCategoryCommandHandlerTests : IClassFixture<CategoryTestFixture>
    {
        private readonly CategoryTestFixture _fixture;

        public DeleteCategoryCommandHandlerTests(CategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        private DeleteCategoryCommand CreateValidDeleteCategoryCommand(int categoryId, string userId)
        {
            return new DeleteCategoryCommand
            {
                Id = categoryId,
                UserId = userId
            };
        }

        [Fact]
        public async Task Handle_ValidCommand_DeletesCategory()
        {
            var testData = _fixture.OneCategory_WithTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();
            
            var command = CreateValidDeleteCategoryCommand(categoryId, _fixture.OneCategory_WithTransactionsUserId);
            var handler = new DeleteCategoryCommandHandler(context);

            await handler.Handle(command, CancellationToken.None);

            var deletedCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            
            var transactions = await context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();

            Assert.Null(deletedCategory);
            Assert.Empty(transactions);
        }


        [Fact]
        public async Task Handle_DeleteDifferentUser_ThrowsArgumentException()
        {
            var testData = _fixture.OneCategory_WithTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();

            var command = CreateValidDeleteCategoryCommand(categoryId, "different-user-id");
            var handler = new DeleteCategoryCommandHandler(context);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_DeleteNonExistentCategory_ThrowsArgumentException()
        {
            using var context = _fixture.CreateContext();
            var nonExistentId = 99999;
            var command = CreateValidDeleteCategoryCommand(nonExistentId, _fixture.OneCategory_WithTransactionsUserId);
            var handler = new DeleteCategoryCommandHandler(context);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_DeleteCategory_OnlyDeletesTransactionsForThatCategory()
        {
            using var context = _fixture.CreateContext();
            var userId = _fixture.MultipleCategories_WithTransactionsUserId;
            
            // Get all categories for this user
            var categories = await context.Categories
                .Where(c => c.IdentityUserId == userId)
                .ToListAsync();

            var shoppingCat = categories.FirstOrDefault(c => c.Name == "Shopping");
            var foodCat = categories.FirstOrDefault(c => c.Name == "Food");

            // Count transactions before delete
            var shoppingTxCountBefore = await context.Transactions
                .Where(t => t.CategoryId == shoppingCat!.Id)
                .CountAsync();

            var foodTxCountBefore = await context.Transactions
                .Where(t => t.CategoryId == foodCat!.Id)
                .CountAsync();

            // Delete shopping category
            var command = CreateValidDeleteCategoryCommand(shoppingCat!.Id, userId);
            var handler = new DeleteCategoryCommandHandler(context);

            await handler.Handle(command, CancellationToken.None);

            // Verify shopping transactions are deleted
            var shoppingTxCountAfter = await context.Transactions
                .Where(t => t.CategoryId == shoppingCat.Id)
                .CountAsync();

            // Verify food transactions are NOT deleted
            var foodTxCountAfter = await context.Transactions
                .Where(t => t.CategoryId == foodCat!.Id)
                .CountAsync();

            Assert.Equal(0, shoppingTxCountAfter);
            Assert.Equal(foodTxCountBefore, foodTxCountAfter);
        }

    }
}
