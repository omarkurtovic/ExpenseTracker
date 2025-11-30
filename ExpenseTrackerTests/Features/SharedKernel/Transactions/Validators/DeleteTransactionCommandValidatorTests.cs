using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands;
using Xunit;

namespace ExpenseTrackerTests.Features.SharedKernel.Transactions.Validators
{
    public class DeleteTransactionCommandValidatorTests
    {
        private DeleteTransactionCommand CreateValidDeleteTransactionCommand()
        {
            return new DeleteTransactionCommand
            {
                UserId = "test-user-id",
                Id = 1
            };
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = CreateValidDeleteTransactionCommand();
            var validator = new DeleteTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidDeleteTransactionCommand();
            command.UserId = string.Empty;
            var validator = new DeleteTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_NullUserId_ReturnsError()
        {
            var command = CreateValidDeleteTransactionCommand();
            command.UserId = null!;
            var validator = new DeleteTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_ZeroId_ReturnsError()
        {
            var command = CreateValidDeleteTransactionCommand();
            command.Id = 0;
            var validator = new DeleteTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeId_ReturnsError()
        {
            var command = CreateValidDeleteTransactionCommand();
            command.Id = -1;
            var validator = new DeleteTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_ValidCommandWithLargeId_ReturnsNoErrors()
        {
            var command = CreateValidDeleteTransactionCommand();
            command.Id = int.MaxValue;
            var validator = new DeleteTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
