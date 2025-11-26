using ExpenseTrackerWebApp.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Commands;

namespace ExpenseTrackerTests.Features.Accounts.Validators
{
    public class EditAccountCommandValidatorTests
    {
        private EditAccountCommand CreateValidEditAccountCommand()
        {
            var accountDto = new AccountDto
            {
                Name = "Checking Account",
                InitialBalance = 1000m,
                Icon = "account",
                Color = "#FF6B6B"
            };

            var result = new EditAccountCommand()
            {
                Id = 1,
                UserId = "test-user-id",
                AccountDto = accountDto
            };
            return result;
        }

        [Fact]
        public void Validate_ValidAccount_ReturnsNoErrors()
        {
            var command = CreateValidEditAccountCommand();
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_IdZero_ReturnsError()
        {
            var command = CreateValidEditAccountCommand();
            command.Id = 0;
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("ID must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidEditAccountCommand();
            command.UserId = string.Empty;
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }


        [Fact]
        public void Validate_MissingName_ReturnsError()
        {
            var command = CreateValidEditAccountCommand();
            command.AccountDto.Name = string.Empty;
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Name is required!", result.Errors.First(e => e.PropertyName == "AccountDto.Name").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingInitialBalance_ReturnsError()
        {
            var command = CreateValidEditAccountCommand();
            command.AccountDto.InitialBalance = null;
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Initial balance is required!", result.Errors.First(e => e.PropertyName == "AccountDto.InitialBalance").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingIcon_ReturnsError()
        {
            var command = CreateValidEditAccountCommand();
            command.AccountDto.Icon = string.Empty;
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Icon is required!", result.Errors.First(e => e.PropertyName == "AccountDto.Icon").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingColor_ReturnsError()
        {
            var command = CreateValidEditAccountCommand();
            command.AccountDto.Color = string.Empty;
            var validator = new EditAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Icon color is required!", result.Errors.First(e => e.PropertyName == "AccountDto.Color").ErrorMessage);
        }
    }
}
