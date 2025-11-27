using ExpenseTrackerWebApp.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Commands;
using ExpenseTrackerWebApp.Features.Accounts.Handlers;
using ExpenseTrackerTests.Features.Accounts.Fixtures;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerWebApp.Features.Accounts.Dtos;

namespace ExpenseTrackerTests.Features.Accounts.Handlers
{
    public class CreateAccountCommandHandlerTests : IClassFixture<AccountTestFixture>
    {
        private readonly AccountTestFixture _fixture;

        public CreateAccountCommandHandlerTests(AccountTestFixture fixture)
        {
            _fixture = fixture;
        }

        private AccountDto CreateValidAccountDto()
        {
            return new AccountDto
            {
                Name = "New Account",
                InitialBalance = 500m,
                Icon = "account",
                Color = "#FF6B6B"
            };
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesAccount()
        {
            using var context = _fixture.CreateContext();
            var accountDto = CreateValidAccountDto();
            
            var command = new CreateAccountCommand()
            {
                UserId = _fixture.NoAccountsUserId,
                AccountDto = accountDto
            };
            var handler = new CreateAccountCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            var createdAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.Id == result && a.IdentityUserId == command.UserId);

            Assert.NotNull(createdAccount);
            Assert.Equal(command.AccountDto.Name, createdAccount!.Name);
            Assert.Equal(command.AccountDto.InitialBalance, createdAccount.InitialBalance);
            Assert.Equal(command.AccountDto.Icon, createdAccount.Icon);
            Assert.Equal(command.AccountDto.Color, createdAccount.Color);
            Assert.Equal(_fixture.NoAccountsUserId, createdAccount.IdentityUserId);
        }
    }
}
