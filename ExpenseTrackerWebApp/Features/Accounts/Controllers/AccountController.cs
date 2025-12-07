using ExpenseTrackerWebApp.Features.Accounts.Commands;
using ExpenseTrackerWebApp.Features.Accounts.Dtos;
using ExpenseTrackerWebApp.Features.Accounts.Queries;
using ExpenseTrackerWebApp.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApp.Features.Accounts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountsController(ISender mediator, ICurrentUserService currentUserService, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var accounts = await _mediator.Send(new GetAccountsWithBalanceQuery() { UserId = user.Id });
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
                var accountDto = await _mediator.Send(new GetAccountQuery() { UserId = _currentUserService.GetUserId(), Id = id });
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
                await _mediator.Send(new CreateAccountCommand() { AccountDto = accountDto, UserId = _currentUserService.GetUserId() });
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
                await _mediator.Send(new EditAccountCommand() { Id = id, AccountDto = accountDto, UserId = _currentUserService.GetUserId() });
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
                await _mediator.Send(new DeleteAccountCommand() { Id = id, UserId = _currentUserService.GetUserId() });
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