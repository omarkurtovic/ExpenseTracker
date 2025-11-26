using ExpenseTrackerWebApp.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Commands;

namespace ExpenseTrackerTests.Features.Accounts.Validators
{
    public class CreateAccountCommandValidatorTests
    {
        private CreateAccountCommand CreateValidCreateAccountCommand()
        {
            var accountDto = new AccountDto
            {
                Name = "Checking Account",
                InitialBalance = 1000m,
                Icon = "account",
                Color = "#FF6B6B"
            };

            var result = new CreateAccountCommand()
            {
                UserId = "test-user-id",
                AccountDto = accountDto
            };
            return result;
        }

        [Fact]
        public void Validate_ValidAccount_ReturnsNoErrors()
        {
            var command = CreateValidCreateAccountCommand();
            var validator = new CreateAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidCreateAccountCommand();
            command.UserId = string.Empty;
            var validator = new CreateAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }


        [Fact]
        public void Validate_MissingName_ReturnsError()
        {
            var command = CreateValidCreateAccountCommand();
            command.AccountDto.Name = string.Empty;
            var validator = new CreateAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Name is required!", result.Errors.First(e => e.PropertyName == "AccountDto.Name").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingInitialBalance_ReturnsError()
        {
            var command = CreateValidCreateAccountCommand();
            command.AccountDto.InitialBalance = null;
            var validator = new CreateAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Initial balance is required!", result.Errors.First(e => e.PropertyName == "AccountDto.InitialBalance").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingIcon_ReturnsError()
        {
            var command = CreateValidCreateAccountCommand();
            command.AccountDto.Icon = string.Empty;
            var validator = new CreateAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Icon is required!", result.Errors.First(e => e.PropertyName == "AccountDto.Icon").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingColor_ReturnsError()
        {
            var command = CreateValidCreateAccountCommand();
            command.AccountDto.Color = string.Empty;
            var validator = new CreateAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Icon color is required!", result.Errors.First(e => e.PropertyName == "AccountDto.Color").ErrorMessage);
        }
    }
}
