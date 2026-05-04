
using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerWebApi.Features.Accounts.Queries;
using ExpenseTrackerWebApi.Features.Budgets.Commands;
using ExpenseTrackerWebApi.Features.Budgets.Queries;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Budgets.Controllers
{

    [ApiController]
    [Route("api/budgets")]
    public class BudgetController(ISender mediator, UserManager<IdentityUser> userManager) : ControllerBase
    {
        private readonly ISender _mediator = mediator;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        [HttpGet]
        public async Task<IActionResult> GetBudgetsWithProgress()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var budgets = await _mediator.Send(new GetBudgetsWithProgressQuery() { UserId = userId });
            return Ok(budgets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudgetById(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            var budgetDto = await _mediator.Send(new GetBudgetQuery() { UserId = userId, Id = id });
            if (budgetDto == null)
            {
                return NotFound();
            }

            return Ok(budgetDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] BudgetDto budgetDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new CreateBudgetCommand() { BudgetDto = budgetDto, UserId = userId });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] BudgetDto budgetDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new EditBudgetCommand() { Id = id, BudgetDto = budgetDto, UserId = userId });
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _mediator.Send(new DeleteBudgetCommand() { Id = id, UserId = userId });
            return Ok("Budget deleted successfully.");
        }
    }
}