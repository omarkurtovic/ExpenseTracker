using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Budgets.Commands;
using ExpenseTrackerWebApp.Features.Budgets.Dtos;
using ExpenseTrackerWebApp.Features.Budgets.Models;

namespace ExpenseTrackerTests.Features.Budgets.Validators
{
    public class CreateBudgetCommandValidatorTests
    {
        
        private CreateBudgetCommand CreateValidCreateBudgetCommand()
        {
            var budgetDto = new BudgetDto();
            budgetDto.Name = "Monthly Budget";
            budgetDto.BudgetType = BudgetType.Monthly;
            budgetDto.Amount = 1000m;
            budgetDto.Description = "This is a test budget.";
            budgetDto.IdentityUserId = "test-user-id";
            var categories = new List<Category>(){new Category { Id = 1, Name = "Food" }};
            var accounts = new List<Account>(){new Account { Id = 1, Name = "Checking Account" }};

            budgetDto.Categories = categories;
            budgetDto.Accounts = accounts;

            var result = new CreateBudgetCommand();
            result.BudgetDto = budgetDto;
            return result;
        }

        [Fact]
        public void Validate_ValidBudget_ReturnsNoErrors()
        {
            var command = CreateValidCreateBudgetCommand();
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.IdentityUserId = string.Empty;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "BudgetDto.IdentityUserId").ErrorMessage);
            
        }

        [Fact]
        public void Validate_MissingName_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.Name = string.Empty;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Name is required!", result.Errors.First(e => e.PropertyName == "BudgetDto.Name").ErrorMessage);
        }


        [Fact]
        public void Validate_MissingBudgetType_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.BudgetType = null;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Budget type is required!", result.Errors.First(e => e.PropertyName == "BudgetDto.BudgetType").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingAmount_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.Amount = null;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Amount is required!", result.Errors.First(e => e.PropertyName == "BudgetDto.Amount").ErrorMessage);
        }

        [Fact]
        public void Validate_ZeroAmount_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.Amount = 0;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Amount must be greater than zero!", result.Errors.First(e => e.PropertyName == "BudgetDto.Amount").ErrorMessage);
        }


        [Fact]
        public void Validate_NoCategories_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.Categories = new List<Category>();
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("At least one category must be selected!", result.Errors.First(e => e.PropertyName == "BudgetDto.Categories").ErrorMessage);
        }

        [Fact]
        public void Validate_NoAccounts_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.Accounts = new List<Account>();
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("At least one account must be selected!", result.Errors.First(e => e.PropertyName == "BudgetDto.Accounts").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeAmount_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.Amount = -500m;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Amount must be greater than zero!", result.Errors.First(e => e.PropertyName == "BudgetDto.Amount").ErrorMessage);
        }

        [Fact]
        public void Validate_NullBudgetType_ReturnsError()
        {
            var command = CreateValidCreateBudgetCommand();
            command.BudgetDto.BudgetType = null;
            var validator = new CreateBudgetCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }
    }
}