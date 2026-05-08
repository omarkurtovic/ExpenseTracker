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
    public class TransactionsController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> GetTransactions([FromBody] TransactionsGridOptionsDto transactionOptions)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var transactions = await _mediator.Send(new GetTransactionsPageDataQuery() { UserId = userId, TransactionOptions = transactionOptions });
            return Ok(transactions);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var transactionDto = await _mediator.Send(new GetTransactionQuery() { UserId = userId, Id = id });
            if (transactionDto == null)
            {
                return NotFound();
            }

            return Ok(transactionDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transactionDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new CreateTransactionCommand() { TransactionDto = transactionDto, UserId = userId });
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction([FromBody] TransactionDto transactionDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new EditTransactionCommand() { Id = (int)transactionDto.Id!, TransactionDto = transactionDto, UserId = userId });
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new DeleteTransactionCommand() { Id = id, UserId = userId });
            return Ok("Transaction deleted successfully.");
        }
    }
}