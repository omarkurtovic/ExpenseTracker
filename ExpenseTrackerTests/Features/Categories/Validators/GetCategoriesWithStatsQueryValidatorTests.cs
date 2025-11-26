using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Queries;

namespace ExpenseTrackerTests.Features.Categories.Validators
{
    public class GetCategoriesWithStatsQueryValidatorTests
    {
        private GetCategoriesWithStatsQuery CreateValidGetCategoriesWithStatsQuery()
        {
            var result = new GetCategoriesWithStatsQuery()
            {
                UserId = "test-user-id",
                Type = null
            };
            return result;
        }

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = CreateValidGetCategoriesWithStatsQuery();
            var validator = new GetCategoriesWithStatsQueryValidator();

            var result = validator.Validate(query);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_ValidQueryWithType_ReturnsNoErrors()
        {
            var query = CreateValidGetCategoriesWithStatsQuery();
            query.Type = TransactionType.Expense;
            var validator = new GetCategoriesWithStatsQueryValidator();

            var result = validator.Validate(query);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var query = CreateValidGetCategoriesWithStatsQuery();
            query.UserId = "";
            var validator = new GetCategoriesWithStatsQueryValidator();

            var result = validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var query = CreateValidGetCategoriesWithStatsQuery();
            query.UserId = null!;
            var validator = new GetCategoriesWithStatsQueryValidator();

            var result = validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_QueryWithIncomeType_ReturnsNoErrors()
        {
            var query = CreateValidGetCategoriesWithStatsQuery();
            query.Type = TransactionType.Income;
            var validator = new GetCategoriesWithStatsQueryValidator();

            var result = validator.Validate(query);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
