using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.Categories.Commands;
using ExpenseTrackerWebApp.Features.Categories.Dtos;

namespace ExpenseTrackerTests.Features.Categories.Validators
{
    public class CreateCategoryCommandValidatorTests
    {
        private CreateCategoryCommand CreateValidCreateCategoryCommand()
        {
            var categoryDto = new CategoryDto
            {
                Name = "Groceries",
                Type = TransactionType.Expense,
                Icon = "shopping_cart",
                Color = "#FF6B6B"
            };

            var result = new CreateCategoryCommand()
            {
                UserId = "test-user-id",
                CategoryDto = categoryDto
            };
            return result;
        }

        [Fact]
        public void Validate_ValidCategory_ReturnsNoErrors()
        {
            var command = CreateValidCreateCategoryCommand();
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.UserId = string.Empty;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingCategoryDto_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.CategoryDto = null!;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Category is required!", result.Errors.First(e => e.PropertyName == "CategoryDto").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingName_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.CategoryDto.Name = string.Empty;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Name is required!", result.Errors.First(e => e.PropertyName == "CategoryDto.Name").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingType_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.CategoryDto.Type = null;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Type is required!", result.Errors.First(e => e.PropertyName == "CategoryDto.Type").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingIcon_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.CategoryDto.Icon = string.Empty;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Icon is required!", result.Errors.First(e => e.PropertyName == "CategoryDto.Icon").ErrorMessage);
        }

        [Fact]
        public void Validate_MissingColor_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.CategoryDto.Color = string.Empty;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Icon color is required!", result.Errors.First(e => e.PropertyName == "CategoryDto.Color").ErrorMessage);
        }

        [Fact]
        public void Validate_NullUserId_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.UserId = null!;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_NullName_ReturnsError()
        {
            var command = CreateValidCreateCategoryCommand();
            command.CategoryDto.Name = null!;
            var validator = new CreateCategoryCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Name is required!", result.Errors.First(e => e.PropertyName == "CategoryDto.Name").ErrorMessage);
        }
    }
}
