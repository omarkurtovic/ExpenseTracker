using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Commands;
using ExpenseTrackerWebApp.Features.Categories.Dtos;
using ExpenseTrackerWebApp.Features.Categories.Handlers;
using ExpenseTrackerTests.Features.Categories.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Categories.Handlers
{
    public class CreateCategoryCommandHandlerTests : IClassFixture<CategoryTestFixture>
    {
        private readonly CategoryTestFixture _fixture;

        public CreateCategoryCommandHandlerTests(CategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        private CreateCategoryCommand CreateValidCreateCategoryCommand(string userId)
        {
            var categoryDto = new CategoryDto
            {
                Name = "Entertainment",
                Type = TransactionType.Expense,
                Icon = "movie",
                Color = "#FF5733"
            };

            var command = new CreateCategoryCommand
            {
                UserId = userId,
                CategoryDto = categoryDto
            };

            return command;
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesCategory()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateCategoryCommand(_fixture.NoCategoriesUserId);
            var handler = new CreateCategoryCommandHandler(context);

            var categoryId = await handler.Handle(command, CancellationToken.None);

            var createdCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            Assert.NotNull(createdCategory);
            Assert.Equal(command.CategoryDto.Name, createdCategory!.Name);
            Assert.Equal(command.CategoryDto.Type, createdCategory.Type);
            Assert.Equal(command.CategoryDto.Icon, createdCategory.Icon);
            Assert.Equal(command.CategoryDto.Color, createdCategory.Color);
            Assert.Equal(command.UserId, createdCategory.IdentityUserId);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsValidCategoryId()
        {
            using var context = _fixture.CreateContext();
            var command = CreateValidCreateCategoryCommand(_fixture.NoCategoriesUserId);
            var handler = new CreateCategoryCommandHandler(context);

            var categoryId = await handler.Handle(command, CancellationToken.None);

            Assert.True(categoryId > 0);
        }

        [Fact]
        public async Task Handle_MultipleCreations_EachHasUniqueId()
        {
            using var context = _fixture.CreateContext();
            var handler = new CreateCategoryCommandHandler(context);

            var command1 = CreateValidCreateCategoryCommand(_fixture.NoCategoriesUserId);
            command1.CategoryDto.Name = "Category 1";

            var command2 = CreateValidCreateCategoryCommand(_fixture.NoCategoriesUserId);
            command2.CategoryDto.Name = "Category 2";

            var id1 = await handler.Handle(command1, CancellationToken.None);
            var id2 = await handler.Handle(command2, CancellationToken.None);

            Assert.NotEqual(id1, id2);
        }

        [Fact]
        public async Task Handle_CreateCategory_IsolatedByUser()
        {
            using var context = _fixture.CreateContext();
            var handler = new CreateCategoryCommandHandler(context);

            // Create category for user 1
            var command1 = CreateValidCreateCategoryCommand(_fixture.NoCategoriesUserId);
            command1.CategoryDto.Name = "User1Category";
            var id1 = await handler.Handle(command1, CancellationToken.None);

            // Create category for user 2
            var command2 = CreateValidCreateCategoryCommand(_fixture.OneCategory_NoTransactionsUserId);
            command2.CategoryDto.Name = "User2Category";
            var id2 = await handler.Handle(command2, CancellationToken.None);

            // Verify user 1 can see their category but not user 2's
            var user1Categories = await context.Categories
                .Where(c => c.IdentityUserId == _fixture.NoCategoriesUserId)
                .ToListAsync();

            var user2Categories = await context.Categories
                .Where(c => c.IdentityUserId == _fixture.OneCategory_NoTransactionsUserId)
                .ToListAsync();

            // User 1 should only see their own category
            Assert.Contains(user1Categories, c => c.Id == id1 && c.Name == "User1Category");
            Assert.DoesNotContain(user1Categories, c => c.Id == id2);

            // User 2 should see their new category plus the existing one
            Assert.Contains(user2Categories, c => c.Id == id2 && c.Name == "User2Category");
        }

        [Fact]
        public async Task Handle_CreateCategoryWithDifferentTypes()
        {
            using var context = _fixture.CreateContext();
            var handler = new CreateCategoryCommandHandler(context);

            // Create expense category
            var expenseCommand = new CreateCategoryCommand
            {
                UserId = _fixture.NoCategoriesUserId,
                CategoryDto = new CategoryDto
                {
                    Name = "Groceries",
                    Type = TransactionType.Expense,
                    Icon = "shopping_cart",
                    Color = "#FF0000"
                }
            };

            // Create income category
            var incomeCommand = new CreateCategoryCommand
            {
                UserId = _fixture.NoCategoriesUserId,
                CategoryDto = new CategoryDto
                {
                    Name = "Salary",
                    Type = TransactionType.Income,
                    Icon = "attach_money",
                    Color = "#00FF00"
                }
            };

            var expenseId = await handler.Handle(expenseCommand, CancellationToken.None);
            var incomeId = await handler.Handle(incomeCommand, CancellationToken.None);

            var expenseCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == expenseId);
            var incomeCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == incomeId);

            Assert.NotNull(expenseCategory);
            Assert.Equal(TransactionType.Expense, expenseCategory!.Type);

            Assert.NotNull(incomeCategory);
            Assert.Equal(TransactionType.Income, incomeCategory!.Type);
        }
    }
}
