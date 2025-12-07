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
    public class AccountsController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly UserManager<IdentityUser> _userManager;

        public AccountsController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var accounts = await _mediator.Send(new GetAccountsWithBalanceQuery() { UserId = userId });
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting accounts: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var accountDto = await _mediator.Send(new GetAccountQuery() { UserId = userId, Id = id });
                if(accountDto == null)
                {
                    return NotFound();
                }

                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting account: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDto accountDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _mediator.Send(new CreateAccountCommand() { AccountDto = accountDto, UserId = userId });
                return Ok();
            }

            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountDto accountDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _mediator.Send(new EditAccountCommand() { Id = id, AccountDto = accountDto, UserId = userId });
                return Ok();
            }

            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving account: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _mediator.Send(new DeleteAccountCommand() { Id = id, UserId = userId });
                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting account: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}