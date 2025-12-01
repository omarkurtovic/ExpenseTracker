using ExpenseTrackerWebApp.Features.Categories.Dtos;
using ExpenseTrackerWebApp.Features.Categories.Handlers;
using ExpenseTrackerWebApp.Features.Categories.Queries;
using ExpenseTrackerTests.Features.Categories.Fixtures;

namespace ExpenseTrackerTests.Features.Categories.Handlers
{
    public class GetCategoryQueryHandlerTests : IClassFixture<CategoryTestFixture>
    {
        private readonly CategoryTestFixture _fixture;

        public GetCategoryQueryHandlerTests(CategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        private GetCategoryQuery CreateValidGetCategoryQuery(int categoryId, string userId)
        {
            return new GetCategoryQuery
            {
                Id = categoryId,
                UserId = userId
            };
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsCategoryDto()
        {
            var testData = _fixture.OneCategory_NoTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();

            var query = CreateValidGetCategoryQuery(categoryId, _fixture.OneCategory_NoTransactionsUserId);
            var handler = new GetCategoryQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<CategoryDto>(result);
        }

        [Fact]
        public async Task Handle_GetDifferentUser_ThrowsUnauthorizedAccessException()
        {
            var testData = _fixture.OneCategory_NoTransactions;
            using var context = _fixture.CreateContext();
            var categoryId = testData.CategoryIds.First();

            var query = CreateValidGetCategoryQuery(categoryId, "different-user-id");
            var handler = new GetCategoryQueryHandler(context);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(query, CancellationToken.None)
            );

            Assert.Equal("Category does not belong to user!", exception.Message);
        }

        [Fact]
        public async Task Handle_GetNonExistentCategory_ThrowsArgumentException()
        {
            using var context = _fixture.CreateContext();
            var nonExistentId = 99999;
            var query = CreateValidGetCategoryQuery(nonExistentId, _fixture.OneCategory_NoTransactionsUserId);
            var handler = new GetCategoryQueryHandler(context);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(query, CancellationToken.None)
            );

            Assert.Equal("Category not found!", exception.Message);
        }
    }
}
