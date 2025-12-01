using ExpenseTrackerWebApp.Features.Register.Commands;
using ExpenseTrackerWebApp.Features.Register.Dtos;

namespace ExpenseTrackerTests.Features.Register
{
    public class RegisterNewUserCommandValidatorTests
    {
        private readonly RegisterNewUserCommandValidator _validator;

        public RegisterNewUserCommandValidatorTests()
        {
            _validator = new RegisterNewUserCommandValidator();
        }


        [Fact]
        public async Task Validate_WhenRegisterDataDtoIsNull_ShouldFail()
        {
            var command = new RegisterNewUserCommand { RegisterDataDto = null };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Account is required"));
        }

        [Fact]
        public async Task Validate_WhenEmailIsEmpty_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Email is required"));
        }

        [Fact]
        public async Task Validate_WhenEmailIsInvalid_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "notanemail",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid email format"));
        }

        [Fact]
        public async Task Validate_WhenEmailIsValid_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);
            Assert.DoesNotContain(result.Errors, e => e.PropertyName.Contains("Email"));
        }

        [Fact]
        public async Task Validate_WhenPasswordIsEmpty_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "",
                    PasswordConfirm = ""
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Password is required"));
        }

        [Fact]
        public async Task Validate_WhenPasswordIsShorterThan6Characters_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@",
                    PasswordConfirm = "Test@"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("at least 6 characters"));
        }

        [Fact]
        public async Task Validate_WhenPasswordIs6Characters_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@1",
                    PasswordConfirm = "Test@1"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.DoesNotContain(result.Errors, e => e.ErrorMessage.Contains("at least 6 characters"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasNoUppercase_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "test@123",
                    PasswordConfirm = "test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("uppercase letter"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasUppercase_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.DoesNotContain(result.Errors, e => e.ErrorMessage.Contains("uppercase letter"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasNoLowercase_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "TEST@123",
                    PasswordConfirm = "TEST@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("lowercase letter"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasLowercase_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.DoesNotContain(result.Errors, e => e.ErrorMessage.Contains("lowercase letter"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasNoDigit_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@abc",
                    PasswordConfirm = "Test@abc"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("digit"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasDigit_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.DoesNotContain(result.Errors, e => e.ErrorMessage.Contains("digit"));
        }


        [Fact]
        public async Task Validate_WhenPasswordHasNoSpecialCharacter_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test1234",
                    PasswordConfirm = "Test1234"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("special character"));
        }

        [Fact]
        public async Task Validate_WhenPasswordHasSpecialCharacter_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.DoesNotContain(result.Errors, e => e.ErrorMessage.Contains("special character"));
        }


        [Fact]
        public async Task Validate_WhenPasswordsDoNotMatch_ShouldFail()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@124"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Passwords do not match"));
        }

        [Fact]
        public async Task Validate_WhenPasswordsMatch_ShouldPass()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "user@example.com",
                    Password = "Test@123",
                    PasswordConfirm = "Test@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_WhenAllFieldsValid_ShouldSucceed()
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = "newuser@example.com",
                    Password = "SecurePass@123",
                    PasswordConfirm = "SecurePass@123"
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("user123@test.com", "Password@1", "Password@1")]
        [InlineData("admin@domain.org", "MyPass@456", "MyPass@456")]
        [InlineData("test+tag@company.net", "Complex!Pass#2024", "Complex!Pass#2024")]
        public async Task Validate_WithVariousValidInputs_ShouldSucceed(string email, string password, string passwordConfirm)
        {
            var command = new RegisterNewUserCommand
            {
                RegisterDataDto = new RegisterDataDto
                {
                    Email = email,
                    Password = password,
                    PasswordConfirm = passwordConfirm
                }
            };

            var result = await _validator.ValidateAsync(command);

            Assert.True(result.IsValid);
        }
    }
}
