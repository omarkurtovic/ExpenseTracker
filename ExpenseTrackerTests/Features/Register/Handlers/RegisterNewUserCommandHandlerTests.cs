using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Features.Register.Commands;
using ExpenseTrackerWebApp.Features.Register.Dtos;
using ExpenseTrackerWebApp.Features.Register.Handlers;
using ExpenseTrackerWebApp.Features.SharedKernel.Commands;
using ExpenseTrackerTests.Features.Register.Fixtures;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ExpenseTrackerTests.Features.Register.Handlers
{
    public class RegisterNewUserCommandHandlerTests : IClassFixture<RegisterTestFixture>
    {
        private readonly RegisterTestFixture _fixture;

        public RegisterNewUserCommandHandlerTests(RegisterTestFixture fixture)
        {
            _fixture = fixture;
        }

        private RegisterDataDto CreateValidRegisterDataDto(string email = "user@example.com")
        {
            return new RegisterDataDto
            {
                Email = email,
                Password = "Test@123",
                PasswordConfirm = "Test@123"
            };
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesUser()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();

            var registerDataDto = CreateValidRegisterDataDto("newuser@example.com");
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            await handler.Handle(command, CancellationToken.None);

            var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
            Assert.NotNull(createdUser);
            Assert.Equal("newuser@example.com", createdUser!.Email);
            Assert.Equal("newuser@example.com", createdUser.UserName);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsResetToDefaultAccountsCommand()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();

            var registerDataDto = CreateValidRegisterDataDto("accounts@example.com");
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            await handler.Handle(command, CancellationToken.None);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<ResetToDefaultAccountsCommand>(c => c.UserId != null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsResetToDefaultCategoriesCommand()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();

            var registerDataDto = CreateValidRegisterDataDto("categories@example.com");
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            await handler.Handle(command, CancellationToken.None);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<ResetToDefaultCategoriesCommand>(c => c.UserId != null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsAddDefaultUserPreferencesCommand()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();

            var registerDataDto = CreateValidRegisterDataDto("preferences@example.com");
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            await handler.Handle(command, CancellationToken.None);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AddDefaultUserPreferencesCommand>(c => c.UserId != null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_ExecutesCommandsInCorrectOrder()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var callOrder = new List<string>();
            var mediatorMock = new Mock<ISender>();

            // Setup mock to track call order
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ResetToDefaultCategoriesCommand>(), It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("ResetCategories"))
                .Returns(Task.CompletedTask);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<AddDefaultUserPreferencesCommand>(), It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("AddPreferences"))
                .Returns(Task.CompletedTask);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<ResetToDefaultAccountsCommand>(), It.IsAny<CancellationToken>()))
                .Callback(() => callOrder.Add("ResetAccounts"))
                .Returns(Task.CompletedTask);

            var registerDataDto = CreateValidRegisterDataDto("ordertest@example.com");
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            await handler.Handle(command, CancellationToken.None);

            Assert.Equal(3, callOrder.Count);
            Assert.Contains("ResetCategories", callOrder);
            Assert.Contains("AddPreferences", callOrder);
            Assert.Contains("ResetAccounts", callOrder);
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            // Create a user first
            var firstUserManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();
            var firstRegisterDataDto = CreateValidRegisterDataDto("duplicate@example.com");
            var firstCommand = new RegisterNewUserCommand { RegisterDataDto = firstRegisterDataDto };
            var handler = new RegisterNewUserCommandHandler(firstUserManager, mediatorMock.Object);

            await handler.Handle(firstCommand, CancellationToken.None);

            // Try to create another user with the same email
            using var context2 = _fixture.CreateContext();
            var secondUserManager = CreateUserManager(context2);
            var secondRegisterDataDto = CreateValidRegisterDataDto("duplicate@example.com");
            var secondCommand = new RegisterNewUserCommand { RegisterDataDto = secondRegisterDataDto };
            var handler2 = new RegisterNewUserCommandHandler(secondUserManager, mediatorMock.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => handler2.Handle(secondCommand, CancellationToken.None));
            Assert.NotNull(exception);
            Assert.Equal("User registration failed", exception.Message);
        }

        [Fact]
        public async Task Handle_DuplicateUsername_ThrowsException()
        {
            using var context = _fixture.CreateContext();

            // Create a user with a specific username
            var firstUserManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();
            var firstRegisterDataDto = CreateValidRegisterDataDto("sameusername@example.com");
            var firstCommand = new RegisterNewUserCommand { RegisterDataDto = firstRegisterDataDto };
            var handler = new RegisterNewUserCommandHandler(firstUserManager, mediatorMock.Object);

            await handler.Handle(firstCommand, CancellationToken.None);

            // Try to create another user with the same username (email is username)
            using var context2 = _fixture.CreateContext();
            var secondUserManager = CreateUserManager(context2);
            var secondRegisterDataDto = CreateValidRegisterDataDto("sameusername@example.com");
            var secondCommand = new RegisterNewUserCommand { RegisterDataDto = secondRegisterDataDto };
            var handler2 = new RegisterNewUserCommandHandler(secondUserManager, mediatorMock.Object);

            var exception = await Assert.ThrowsAsync<Exception>(() => handler2.Handle(secondCommand, CancellationToken.None));
            Assert.NotNull(exception);
            Assert.Equal("User registration failed", exception.Message);
        }

        [Fact]
        public async Task Handle_ValidCommand_UsesEmailAsUsername()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();

            var email = "testuser@domain.com";
            var registerDataDto = CreateValidRegisterDataDto(email);
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            await handler.Handle(command, CancellationToken.None);

            var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            Assert.NotNull(createdUser);
            Assert.Equal(email, createdUser!.UserName);
        }

        [Fact]
        public async Task Handle_RegistrationSucceeds_ReturnsUnitValue()
        {
            using var context = _fixture.CreateContext();
            var userManager = CreateUserManager(context);
            var mediatorMock = CreateMockMediator();

            var registerDataDto = CreateValidRegisterDataDto("success@example.com");
            var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
            var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_MultipleValidRegistrations_CreatesMultipleUsers()
        {
            // Use a fresh fixture for this test to ensure clean database
            var freshFixture = new RegisterTestFixture();
            try
            {
                var emails = new[] { "multi1@example.com", "multi2@example.com", "multi3@example.com" };
                var mediatorMock = CreateMockMediator();

                foreach (var email in emails)
                {
                    using var contextForUser = freshFixture.CreateContext();
                    var userManager = CreateUserManager(contextForUser);
                    var registerDataDto = CreateValidRegisterDataDto(email);
                    var command = new RegisterNewUserCommand { RegisterDataDto = registerDataDto };
                    var handler = new RegisterNewUserCommandHandler(userManager, mediatorMock.Object);

                    await handler.Handle(command, CancellationToken.None);
                }

                // Verify all users were created by checking each one individually
                using var finalContext = freshFixture.CreateContext();
                foreach (var email in emails)
                {
                    var user = await finalContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                    Assert.NotNull(user);
                    Assert.Equal(email, user!.Email);
                    Assert.Equal(email, user.UserName);
                }
            }
            finally
            {
                freshFixture.Dispose();
            }
        }

        // Helper methods
        private Mock<ISender> CreateMockMediator()
        {
            var mediatorMock = new Mock<ISender>();

            // Setup default returns
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ResetToDefaultAccountsCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<ResetToDefaultCategoriesCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<AddDefaultUserPreferencesCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return mediatorMock;
        }

        private UserManager<IdentityUser> CreateUserManager(AppDbContext context)
        {
            // Create a real UserManager that uses the actual database
            var userStore = new UserStore<IdentityUser>(context);
            var lookupNormalizer = new UpperInvariantLookupNormalizer();
            var identityErrorDescriber = new IdentityErrorDescriber();
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var userValidators = new List<IUserValidator<IdentityUser>> { new UserValidator<IdentityUser>() };
            var passwordValidators = new List<IPasswordValidator<IdentityUser>> { new PasswordValidator<IdentityUser>() };
            var logger = new Mock<ILogger<UserManager<IdentityUser>>>().Object;

            return new UserManager<IdentityUser>(
                userStore,
                new Mock<IOptions<IdentityOptions>>().Object,
                passwordHasher,
                userValidators,
                passwordValidators,
                lookupNormalizer,
                identityErrorDescriber,
                null!,
                logger);
        }
    }
}
