using ExpenseTrackerWebApp.Features.Accounts.Queries;

namespace ExpenseTrackerTests.Features.Accounts.Validators
{
    public class GetAccountQueryValidatorTests
    {
        private GetAccountQuery CreateValidGetAccountQuery()
        {
            var result = new GetAccountQuery
            {
                Id = 1,
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var command = CreateValidGetAccountQuery();
            var validator = new GetAccountQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingId_ReturnsError()
        {
            var command = CreateValidGetAccountQuery();
            command.Id = 0;
            var validator = new GetAccountQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidGetAccountQuery();
            command.UserId = "";
            var validator = new GetAccountQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidGetAccountQuery();
            command.UserId = null!;
            var validator = new GetAccountQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}
