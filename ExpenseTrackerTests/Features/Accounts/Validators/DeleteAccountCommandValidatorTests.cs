using ExpenseTrackerWebApp.Features.Accounts.Commands;

namespace ExpenseTrackerTests.Features.Accounts.Validators
{
    public class DeleteAccountCommandValidatorTests
    {
        private DeleteAccountCommand CreateValidDeleteAccountCommand()
        {
            var result = new DeleteAccountCommand()
            {
                Id = 1,
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidAccount_ReturnsNoErrors()
        {
            var command = CreateValidDeleteAccountCommand();
            var validator = new DeleteAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingId_ReturnsError()
        {
            var command = CreateValidDeleteAccountCommand();
            command.Id = 0;
            var validator = new DeleteAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidDeleteAccountCommand();
            command.UserId = "";
            var validator = new DeleteAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidDeleteAccountCommand();
            command.UserId = null!;
            var validator = new DeleteAccountCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}
