using System.Transactions;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using ExpenseTrackerWebApi.Features.Tags.Queries;
using ExpenseTrackerWebApi.Features.Transactions.Commands;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Transactions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionsController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> GetTransactions([FromBody] TransactionsGridOptionsDto transactionOptions)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var transactions = await _mediator.Send(new GetTransactionsPageDataQuery() { UserId = userId, TransactionOptions = transactionOptions });
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transactions: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var transactionDto = await _mediator.Send(new GetTransactionQuery() { UserId = userId, Id = id });
                if(transactionDto == null)
                {
                    return NotFound();
                }

                return Ok(transactionDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transaction: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                await _mediator.Send(new CreateTransactionCommand() { TransactionDto = transactionDto, UserId = userId });
                return Ok();
            }

            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transaction: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction([FromBody] TransactionDto transactionDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                await _mediator.Send(new EditTransactionCommand() { Id = (int)transactionDto.Id!, TransactionDto = transactionDto, UserId = userId });
                return Ok();
            }

            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transaction: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                await _mediator.Send(new DeleteTransactionCommand() { Id = id, UserId = userId });
                return Ok("Transaction deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting transaction: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}