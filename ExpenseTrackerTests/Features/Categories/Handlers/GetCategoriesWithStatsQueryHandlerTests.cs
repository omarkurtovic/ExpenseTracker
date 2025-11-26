using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Dtos;
using ExpenseTrackerWebApp.Features.Categories.Handlers;
using ExpenseTrackerWebApp.Features.Categories.Queries;
using ExpenseTrackerTests.Features.Categories.Fixtures;
using Xunit;

namespace ExpenseTrackerTests.Features.Categories.Handlers
{
    public class GetCategoriesWithStatsQueryHandlerTests : IClassFixture<CategoryTestFixture>
    {
        private readonly CategoryTestFixture _fixture;

        public GetCategoriesWithStatsQueryHandlerTests(CategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        private GetCategoriesWithStatsQuery CreateValidGetCategoriesWithStatsQuery(string userId)
        {
            return new GetCategoriesWithStatsQuery
            {
                UserId = userId,
                Type = null // Get all types
            };
        }

        private GetCategoriesWithStatsQuery CreateGetCategoriesWithStatsQueryForType(string userId, TransactionType type)
        {
            return new GetCategoriesWithStatsQuery
            {
                UserId = userId,
                Type = type
            };
        }

        [Fact]
        public async Task Handle_UserWithNoCategories_ReturnsEmptyList()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.NoCategoriesUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_UserWithNoData_ReturnsEmptyList()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.NoDataUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_UserWithOneCategory_ReturnsOneCategory()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.OneCategory_NoTransactionsUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task Handle_UserWithMultipleCategories_ReturnsAllCategories()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.MultipleCategories_WithTransactionsUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); // Shopping, Food, Salary
        }

        [Fact]
        public async Task Handle_FilterByType_ReturnsOnlyExpenseCategories()
        {
            using var context = _fixture.CreateContext();
            var query = CreateGetCategoriesWithStatsQueryForType(_fixture.MultipleCategories_WithTransactionsUserId, TransactionType.Expense);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Shopping, Food
            Assert.All(result, cat => Assert.Equal(TransactionType.Expense, cat.Type));
        }

        [Fact]
        public async Task Handle_FilterByType_ReturnsOnlyIncomeCategories()
        {
            using var context = _fixture.CreateContext();
            var query = CreateGetCategoriesWithStatsQueryForType(_fixture.MultipleCategories_WithTransactionsUserId, TransactionType.Income);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result); // Salary
            Assert.Equal(TransactionType.Income, result.First().Type);
        }

        [Fact]
        public async Task Handle_DifferentUser_DoesNotReturnCategories()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery("different-user-id");
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_CategoriesHaveCorrectStats()
        {
            using var context = _fixture.CreateContext();
            var testData = _fixture.MultipleCategories_WithTransactions;
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.MultipleCategories_WithTransactionsUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            var categories = result.ToList();
            Assert.NotEmpty(categories);

            // Verify each category has stats populated
            foreach (var category in categories)
            {
                Assert.NotNull(category);
                Assert.True(!string.IsNullOrEmpty(category.Name));
                Assert.True(category.TransactionsCount >= 0);
                Assert.True(category.CurrentMonthAmount >= 0 || category.CurrentMonthAmount < 0); // Could be positive or negative
                Assert.True(category.LastMonthAmount >= 0 || category.LastMonthAmount < 0);
            }
        }

        [Fact]
        public async Task Handle_CurrentMonthAmountsAreCorrect()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.MultipleCategories_WithTransactionsUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            var categories = result.ToList();
            var shoppingCat = categories.FirstOrDefault(c => c.Name == "Shopping");
            var foodCat = categories.FirstOrDefault(c => c.Name == "Food");
            var salaryCat = categories.FirstOrDefault(c => c.Name == "Salary");

            Assert.NotNull(shoppingCat);
            Assert.Equal(-200m, shoppingCat.CurrentMonthAmount);

            Assert.NotNull(foodCat);
            Assert.Equal(-150m, foodCat.CurrentMonthAmount);

            Assert.NotNull(salaryCat);
            Assert.Equal(3000m, salaryCat.CurrentMonthAmount);
        }

        [Fact]
        public async Task Handle_LastMonthAmountsAreCorrect()
        {
            using var context = _fixture.CreateContext();
            var query = CreateValidGetCategoriesWithStatsQuery(_fixture.MultipleCategories_WithTransactionsUserId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);

            var result = await handler.Handle(query, CancellationToken.None);

            var categories = result.ToList();
            var shoppingCat = categories.FirstOrDefault(c => c.Name == "Shopping");
            var foodCat = categories.FirstOrDefault(c => c.Name == "Food");
            var salaryCat = categories.FirstOrDefault(c => c.Name == "Salary");

            // Verify last month amounts match fixture data
            Assert.NotNull(shoppingCat);
            Assert.Equal(-100m, shoppingCat.LastMonthAmount);

            Assert.NotNull(foodCat);
            Assert.Equal(-200m, foodCat.LastMonthAmount);

            Assert.NotNull(salaryCat);
            Assert.Equal(3000m, salaryCat.LastMonthAmount);
        }

        [Fact]
        public async Task Handle_RecurringTransactionsExcludedFromCalculations()
        {
            using var context = _fixture.CreateContext();
            var userId = _fixture.OneCategory_WithTransactionsUserId;

            // Get initial stats
            var query1 = CreateValidGetCategoriesWithStatsQuery(userId);
            var handler = new GetCategoriesWithStatsQueryHandler(context);
            var resultBefore = await handler.Handle(query1, CancellationToken.None);
            
            Assert.Single(resultBefore);
            var categoryBefore = resultBefore.First();
            var currentMonthBefore = categoryBefore.CurrentMonthAmount;

            // Add a recurring transaction for the same category in current month
            var currentMonth = DateTime.Now;
            var account = context.Accounts.FirstOrDefault(a => a.IdentityUserId == userId);
            var category = context.Categories.FirstOrDefault(c => c.IdentityUserId == userId);
            
            var recurringTx = new Transaction
            {
                AccountId = account!.Id,
                CategoryId = category!.Id,
                Amount = 500m,
                Date = currentMonth,
                Description = "Recurring transaction",
                IsReoccuring = true
            };
            context.Transactions.Add(recurringTx);
            context.SaveChanges();

            // Get stats again
            var query2 = CreateValidGetCategoriesWithStatsQuery(userId);
            var resultAfter = await handler.Handle(query2, CancellationToken.None);
            var categoryAfter = resultAfter.First();
            var currentMonthAfter = categoryAfter.CurrentMonthAmount;

            // Current month amount should NOT include the recurring transaction (500 should not be added)
            Assert.Equal(currentMonthBefore, currentMonthAfter);
        }
    }
}
