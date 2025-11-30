using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Commands;
using ExpenseTrackerWebApp.Features.SharedKernel.Transactions.Dtos;
using Xunit;

namespace ExpenseTrackerTests.Features.SharedKernel.Transactions.Validators
{
    public class CreateTransactionCommandValidatorTests
    {
        private CreateTransactionCommand CreateValidCreateTransactionCommand()
        {
            var transactionDto = new TransactionDto
            {
                Amount = 50m,
                Date = DateTime.Today,
                Time = TimeSpan.FromHours(14),
                AccountId = 1,
                CategoryId = 1,
                Description = "Test transaction",
                IsReoccuring = false,
                TagIds = new List<int>(),
                TransactionType = TransactionType.Expense
            };

            return new CreateTransactionCommand
            {
                UserId = "test-user-id",
                TransactionDto = transactionDto
            };
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsNoErrors()
        {
            var command = CreateValidCreateTransactionCommand();
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_MissingUserId_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.UserId = string.Empty;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_NullUserId_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.UserId = null!;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("User ID is required!", result.Errors.First(e => e.PropertyName == "UserId").ErrorMessage);
        }

        [Fact]
        public void Validate_NullTransactionDto_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto = null!;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Transaction is required!", result.Errors.First(e => e.PropertyName == "TransactionDto").ErrorMessage);
        }

        [Fact]
        public void Validate_ZeroAmount_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.Amount = 0;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "TransactionDto.Amount");
        }

        [Fact]
        public void Validate_NegativeAmount_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.Amount = -50m;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Amount must be greater than 0!", result.Errors.First(e => e.PropertyName == "TransactionDto.Amount").ErrorMessage);
        }

        [Fact]
        public void Validate_NullDate_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.Date = null;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Date is required!", result.Errors.First(e => e.PropertyName == "TransactionDto.Date").ErrorMessage);
        }

        [Fact]
        public void Validate_NullTime_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.Time = null;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "TransactionDto.Time");
        }

        [Fact]
        public void Validate_ZeroAccountId_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.AccountId = 0;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "TransactionDto.AccountId");
        }

        [Fact]
        public void Validate_ZeroCategoryId_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.CategoryId = 0;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "TransactionDto.CategoryId");
        }

        [Fact]
        public void Validate_ReoccuringWithoutFrequency_ReturnsError()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.IsReoccuring = true;
            command.TransactionDto.ReoccuranceFrequency = null;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Reoccurance Frequency is required!", result.Errors.First(e => e.PropertyName == "TransactionDto.ReoccuranceFrequency").ErrorMessage);
        }

        [Fact]
        public void Validate_NonReoccuringWithoutFrequency_ReturnsNoErrors()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.IsReoccuring = false;
            command.TransactionDto.ReoccuranceFrequency = null;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_ValidReoccuringTransaction_ReturnsNoErrors()
        {
            var command = CreateValidCreateTransactionCommand();
            command.TransactionDto.IsReoccuring = true;
            command.TransactionDto.ReoccuranceFrequency = ReoccuranceFrequency.Monthly;
            var validator = new CreateTransactionCommandValidator();

            var result = validator.Validate(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
