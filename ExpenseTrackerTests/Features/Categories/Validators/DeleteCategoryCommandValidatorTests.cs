using ExpenseTrackerWebApp.Features.Categories.Commands;

namespace ExpenseTrackerTests.Features.Categories.Validators
{
    public class DeleteCategoryCommandValidatorTests
    {
        private DeleteCategoryCommand CreateValidDeleteCategoryCommand()
        {
            var result = new DeleteCategoryCommand()
            {
                Id = 1,
                UserId = "test-user-id"
            };
            return result;
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = CreateValidDeleteCategoryCommand();
            var validator = new DeleteCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingId_ReturnsError()
        {
            var command = CreateValidDeleteCategoryCommand();
            command.Id = 0;
            var validator = new DeleteCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeId_ReturnsError()
        {
            var command = CreateValidDeleteCategoryCommand();
            command.Id = -1;
            var validator = new DeleteCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Id must be greater than zero!", result.Errors.First(e => e.PropertyName == "Id").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdEmpty_ReturnsError()
        {
            var command = CreateValidDeleteCategoryCommand();
            command.UserId = "";
            var validator = new DeleteCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_UserIdNull_ReturnsError()
        {
            var command = CreateValidDeleteCategoryCommand();
            command.UserId = null!;
            var validator = new DeleteCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }
    }
}
