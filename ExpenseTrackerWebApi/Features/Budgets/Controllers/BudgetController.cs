
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
    public class BudgetController : ControllerBase
    {
        private readonly ISender _mediator;   
        private readonly UserManager<IdentityUser> _userManager;

        public BudgetController(ISender mediator, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }



        [HttpGet]
        public async Task<IActionResult> GetBudgetsWithProgress()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var budgets = await _mediator.Send(new GetBudgetsWithProgressQuery() { UserId = userId });
                return Ok(budgets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting budgets with progress: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudgetById(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                var budgetDto = await _mediator.Send(new GetBudgetQuery() { UserId = userId, Id = id });
                if(budgetDto == null)
                {
                    return NotFound();
                }

                return Ok(budgetDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting budget: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] BudgetDto budgetDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                await _mediator.Send(new CreateBudgetCommand() { BudgetDto = budgetDto, UserId = userId });
                return Ok();
            }

            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving budget: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] BudgetDto budgetDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                await _mediator.Send(new EditBudgetCommand() { Id = id, BudgetDto = budgetDto, UserId = userId });
                return Ok();
            }

            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving budget: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
                await _mediator.Send(new DeleteBudgetCommand() { Id = id, UserId = userId });
                return Ok("Budget deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting budget: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}