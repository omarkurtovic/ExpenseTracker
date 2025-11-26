using ExpenseTrackerWebApp.Features.Categories.Queries;

namespace ExpenseTrackerTests.Features.Categories.Validators
{
    public class GetCategoryQueryValidatorTests
    {
        private GetCategoryQuery CreateValidGetCategoryQuery()
        {
            var result = new GetCategoryQuery()
            {
                Id = 1,
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var query = CreateValidGetCategoryQuery();
            var validator = new GetCategoryQueryValidator();

            var result = validator.Validate(query);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingId_ReturnsError()
        {
            var query = CreateValidGetCategoryQuery();
            query.Id = 0;
            var validator = new GetCategoryQueryValidator();

            var result = validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeId_ReturnsError()
        {
            var query = CreateValidGetCategoryQuery();
            query.Id = -1;
            var validator = new GetCategoryQueryValidator();

            var result = validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var query = CreateValidGetCategoryQuery();
            query.UserId = "";
            var validator = new GetCategoryQueryValidator();

            var result = validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var query = CreateValidGetCategoryQuery();
            query.UserId = null!;
            var validator = new GetCategoryQueryValidator();

            var result = validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}
