using System.ComponentModel.Design;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Queries;

namespace ExpenseTrackerTests.Features.Budgets.Validators
{
    public class GetBudgetsQueryValidatorTests
    {
        
        private GetBudgetsQuery CreateValidGetBudgetsQuery()
        {
            var result = new GetBudgetsQuery()
            {
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidQuery_ReturnsNoErrors()
        {
            var command = CreateValidGetBudgetsQuery();
            var validator = new GetBudgetsQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidGetBudgetsQuery();
            command.UserId = "";
            var validator = new GetBudgetsQueryValidator();
            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidGetBudgetsQuery();
            command.UserId = null!;
            var validator = new GetBudgetsQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}