using ExpenseTrackerWebApi.Features.Accounts.Commands;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Accounts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var accounts = await _mediator.Send(new GetAccountsQuery() { UserId = userId });
            return Ok(accounts);
        }


        [HttpGet]
        [Route("with-balances")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetAccountsWithBalance()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var accounts = await _mediator.Send(new GetAccountsWithBalanceQuery() { UserId = userId });
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var accountDto = await _mediator.Send(new GetAccountQuery() { UserId = userId, Id = id });
            if (accountDto == null)
            {
                return NotFound();
            }

            return Ok(accountDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDto accountDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new CreateAccountCommand() { AccountDto = accountDto, UserId = userId });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountDto accountDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new EditAccountCommand() { Id = id, AccountDto = accountDto, UserId = userId });
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new DeleteAccountCommand() { Id = id, UserId = userId });
            return Ok("Account deleted successfully.");
        }
    }
}