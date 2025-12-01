using ExpenseTrackerWebApp.Features.Budgets.Queries;

namespace ExpenseTrackerTests.Features.Budgets.Validators
{
    public class GetBudgetsWithProgressQueryValidatorTests
    {
        
        private GetBudgetsWithProgressQuery CreateValidGetBudgetsWithProgressQuery()
        {
            var result = new GetBudgetsWithProgressQuery()
            {
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var command = CreateValidGetBudgetsWithProgressQuery();
            var validator = new GetBudgetsWithProgressQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidGetBudgetsWithProgressQuery();
            command.UserId = "";
            var validator = new GetBudgetsWithProgressQueryValidator();
            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidGetBudgetsWithProgressQuery();
            command.UserId = null!;
            var validator = new GetBudgetsWithProgressQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}