using ExpenseTrackerWebApp.Features.Accounts.Queries;

namespace ExpenseTrackerTests.Features.Accounts.Validators
{
    public class GetAccountsWithBalanceQueryValidatorTests
    {
        private GetAccountsWithBalanceQuery CreateValidGetAccountsWithBalanceQuery()
        {
            var result = new GetAccountsWithBalanceQuery
            {
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var command = CreateValidGetAccountsWithBalanceQuery();
            var validator = new GetAccountsWithBalanceQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidGetAccountsWithBalanceQuery();
            command.UserId = "";
            var validator = new GetAccountsWithBalanceQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidGetAccountsWithBalanceQuery();
            command.UserId = null!;
            var validator = new GetAccountsWithBalanceQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}
