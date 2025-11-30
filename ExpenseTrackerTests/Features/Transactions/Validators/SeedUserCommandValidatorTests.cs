using ExpenseTrackerWebApp.Features.Transactions.Commands;
using ExpenseTrackerWebApp.Features.Transactions.Dtos;
using Xunit;

namespace ExpenseTrackerTests.Features.Transactions.Validators
{
    public class SeedUserCommandValidatorTests
    {
        private SeedUserCommand CreateValidSeedUserCommand()
        {
            var options = new SeedDataOptionsDto
            {
                NumberOfTransaction = 1000,
                TransactionMinAmount = 10,
                TransactionMaxAmount = 500,
                TransactionStartDate = DateTime.Today.AddMonths(-6),
                TransactionEndDate = DateTime.Today,
                MaxNumberOfTags = 3
            };
            var command = new SeedUserCommand()
            {
                UserId = "test-user-id",
                Options = options
            };
            return command;
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = CreateValidSeedUserCommand();
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.UserId = string.Empty;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_NullUserId_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.UserId = null!;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_NullOptions_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options = null!;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Seed data options are required!", result.Errors.First(e => e.PropertyName == "Options").ErrorMessage);
        }

        [Fact]
        public void Validate_NullNumberOfTransactions_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.NumberOfTransaction = 0; 
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Options.NumberOfTransaction");
        }

        [Fact]
        public void Validate_NullTransactionMinAmount_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMinAmount = 0;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Options.TransactionMinAmount");
        }

        [Fact]
        public void Validate_NullTransactionMaxAmount_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMaxAmount = 0;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Options.TransactionMaxAmount");
        }

        [Fact]
        public void Validate_NullMaxNumberOfTags_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.MaxNumberOfTags = -1; 
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Options.MaxNumberOfTags");
        }

        [Fact]
        public void Validate_ZeroNumberOfTransactions_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.NumberOfTransaction = 0;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Number of transactions must be greater than 0!", result.Errors.First(e => e.PropertyName == "Options.NumberOfTransaction").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeNumberOfTransactions_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.NumberOfTransaction = -100;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Number of transactions must be greater than 0!", result.Errors.First(e => e.PropertyName == "Options.NumberOfTransaction").ErrorMessage);
        }

        [Fact]
        public void Validate_NumberOfTransactionsGreaterThan10000_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.NumberOfTransaction = 10000;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Number of transactions must be less than 10000!", result.Errors.First(e => e.PropertyName == "Options.NumberOfTransaction").ErrorMessage);
        }

        [Fact]
        public void Validate_ZeroTransactionMinAmount_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMinAmount = 0;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Transaction minimum amount must be greater than 0!", result.Errors.First(e => e.PropertyName == "Options.TransactionMinAmount").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeTransactionMinAmount_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMinAmount = -50;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Transaction minimum amount must be greater than 0!", result.Errors.First(e => e.PropertyName == "Options.TransactionMinAmount").ErrorMessage);
        }

        [Fact]
        public void Validate_MaxAmountLessThanMinAmount_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMinAmount = 100;
            command.Options.TransactionMaxAmount = 50;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            var error = result.Errors.FirstOrDefault(e => e.PropertyName == "Options.TransactionMaxAmount");
            Assert.NotNull(error);
        }

        [Fact]
        public void Validate_MaxAmountEqualToMinAmount_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMinAmount = 100;
            command.Options.TransactionMaxAmount = 100;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            var error = result.Errors.FirstOrDefault(e => e.PropertyName == "Options.TransactionMaxAmount");
            Assert.NotNull(error);
        }

        [Fact]
        public void Validate_TransactionMaxAmountGreaterThan1000000_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionMaxAmount = 1000000;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Transaction maximum amount must be less than 1,000,000!", result.Errors.First(e => e.PropertyName == "Options.TransactionMaxAmount").ErrorMessage);
        }

        [Fact]
        public void Validate_StartDateAfterEndDate_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionStartDate = DateTime.Today;
            command.Options.TransactionEndDate = DateTime.Today.AddDays(-5);
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Transaction start date must be before end date!", result.Errors.First(e => e.PropertyName == "Options.TransactionStartDate").ErrorMessage);
        }

        [Fact]
        public void Validate_StartDateEqualToEndDate_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.TransactionStartDate = DateTime.Today;
            command.Options.TransactionEndDate = DateTime.Today;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Transaction start date must be before end date!", result.Errors.First(e => e.PropertyName == "Options.TransactionStartDate").ErrorMessage);
        }

        [Fact]
        public void Validate_NegativeMaxNumberOfTags_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.MaxNumberOfTags = -1;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Maximum number of tags must be at least 0!", result.Errors.First(e => e.PropertyName == "Options.MaxNumberOfTags").ErrorMessage);
        }

        [Fact]
        public void Validate_MaxNumberOfTagsGreaterThan10_ReturnsError()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.MaxNumberOfTags = 11;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Maximum number of tags must be at most 10!", result.Errors.First(e => e.PropertyName == "Options.MaxNumberOfTags").ErrorMessage);
        }

        [Fact]
        public void Validate_ValidBoundaryValues_ReturnsNoErrors()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.NumberOfTransaction = 9999;
            command.Options.TransactionMinAmount = 1;
            command.Options.TransactionMaxAmount = 999999;
            command.Options.MaxNumberOfTags = 10;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_ValidMinimumValues_ReturnsNoErrors()
        {
            var command = CreateValidSeedUserCommand();
            command.Options.NumberOfTransaction = 1;
            command.Options.TransactionMinAmount = 1;
            command.Options.TransactionMaxAmount = 2;
            command.Options.MaxNumberOfTags = 0;
            var validator = new SeedUserQueryValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
