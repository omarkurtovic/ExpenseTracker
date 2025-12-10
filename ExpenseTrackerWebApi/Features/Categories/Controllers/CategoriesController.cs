using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Categories.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _mediator;

        public CategoriesController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var categories = await _mediator.Send(new GetCategoriesQuery { UserId = userId! });
                return Ok(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("with-stats")]
        public async Task<IActionResult> GetCategoriesWithStats()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var categories = await _mediator.Send(new GetCategoriesWithStatsQuery { UserId = userId! });
                return Ok(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var categoryDto = await _mediator.Send(new GetCategoryQuery { UserId = userId!, Id = id });
                if (categoryDto == null)
                {
                    return NotFound();
                }

                return Ok(categoryDto);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting category: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var id = await _mediator.Send(new CreateCategoryCommand { CategoryDto = categoryDto, UserId = userId! });
                return Ok(new { Id = id });
            }
            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving category: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _mediator.Send(new EditCategoryCommand { Id = id, CategoryDto = categoryDto, UserId = userId! });
                return Ok();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (FluentValidation.ValidationException vex)
            {
                var errors = string.Join(", ", vex.Errors.Select(e => e.ErrorMessage));
                return BadRequest(errors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving category: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                await _mediator.Send(new DeleteCategoryCommand { Id = id, UserId = userId! });
                return Ok("Category deleted successfully.");
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
