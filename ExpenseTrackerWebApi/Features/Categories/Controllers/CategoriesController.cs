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
    public class CategoriesController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var categories = await _mediator.Send(new GetCategoriesQuery { UserId = userId! });
            return Ok(categories);
        }

        [HttpGet]
        [Route("with-stats")]
        public async Task<IActionResult> GetCategoriesWithStats()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var categories = await _mediator.Send(new GetCategoriesWithStatsQuery { UserId = userId! });
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var categoryDto = await _mediator.Send(new GetCategoryQuery { UserId = userId!, Id = id });
            if (categoryDto == null)
            {
                return NotFound();
            }

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var id = await _mediator.Send(new CreateCategoryCommand { CategoryDto = categoryDto, UserId = userId! });
            return Ok(new { Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _mediator.Send(new EditCategoryCommand { Id = id, CategoryDto = categoryDto, UserId = userId! });
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _mediator.Send(new DeleteCategoryCommand { Id = id, UserId = userId! });
            return Ok("Category deleted successfully.");
        }
    }
}
