using System.ComponentModel.Design;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;

namespace ExpenseTrackerTests.Features.Budgets.Validators
{
    public class DeleteBudgetCommandValidatorTests
    {
        
        private DeleteBudgetCommand CreateValidDeleteBudgetCommand()
        {
            var result = new DeleteBudgetCommand();
            result.Id = 1;
            result.UserId = "test-user-id";
            return result;
        }

        [Fact]
        public void Validate_ValidBudget_ReturnsNoErrors()
        {
            var command = CreateValidDeleteBudgetCommand();
            var validator = new DeleteBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidDeleteBudgetCommand();
            command.UserId = "";
            var validator = new DeleteBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidDeleteBudgetCommand();
            command.UserId = null!;
            var validator = new DeleteBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}