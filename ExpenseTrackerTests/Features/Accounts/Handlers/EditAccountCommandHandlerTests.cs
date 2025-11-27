using ExpenseTrackerWebApp.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Commands;
using ExpenseTrackerWebApp.Features.Accounts.Handlers;
using ExpenseTrackerTests.Features.Accounts.Fixtures;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerWebApp.Features.Accounts.Dtos;

namespace ExpenseTrackerTests.Features.Accounts.Handlers
{
    public class EditAccountCommandHandlerTests : IClassFixture<AccountTestFixture>
    {
        private readonly AccountTestFixture _fixture;

        public EditAccountCommandHandlerTests(AccountTestFixture fixture)
        {
            _fixture = fixture;
        }

        private AccountDto CreateValidAccountDto()
        {
            return new AccountDto
            {
                Name = "Updated Account",
                InitialBalance = 2000m,
                Icon = "savings",
                Color = "#4ECDC4"
            };
        }

        [Fact]
        public async Task Handle_ValidCommand_EditsAccount()
        {
            using var context = _fixture.CreateContext();
            var accountDto = CreateValidAccountDto();
            var accountId = _fixture.AccountNoTransactions.AccountIds[0];
            
            var command = new EditAccountCommand()
            {
                Id = accountId,
                UserId = _fixture.AccountNoTransactionsUserId,
                AccountDto = accountDto
            };
            var handler = new EditAccountCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            var editedAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.Id == result);

            Assert.NotNull(editedAccount);
            Assert.Equal(command.AccountDto.Name, editedAccount!.Name);
            Assert.Equal(command.AccountDto.InitialBalance, editedAccount.InitialBalance);
            Assert.Equal(command.AccountDto.Icon, editedAccount.Icon);
            Assert.Equal(command.AccountDto.Color, editedAccount.Color);
        }

        [Fact]
        public async Task Handle_AccountNotFound_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var accountDto = CreateValidAccountDto();
            
            var command = new EditAccountCommand()
            {
                Id = 999,
                UserId = _fixture.AccountNoTransactionsUserId,
                AccountDto = accountDto
            };
            var handler = new EditAccountCommandHandler(context);

            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_AccountDoesNotBelongToUser_ThrowsException()
        {
            using var context = _fixture.CreateContext();
            var accountDto = CreateValidAccountDto();
            var accountId = _fixture.AccountNoTransactions.AccountIds[0];
            
            var command = new EditAccountCommand()
            {
                Id = accountId,
                UserId = _fixture.MultipleAccountsUserId,
                AccountDto = accountDto
            };
            var handler = new EditAccountCommandHandler(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None)
            );
        }
    }
}
