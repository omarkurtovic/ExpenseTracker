using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerWebApi.Features.Tags.Commands;
using ExpenseTrackerWebApi.Features.Tags.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerWebApi.Features.Tags.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TagsController(ISender mediator) : ControllerBase
    {
        private readonly ISender _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var tags = await _mediator.Send(new GetTagsQuery { UserId = userId! });
            return Ok(tags);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var tagDto = await _mediator.Send(new GetTagQuery { UserId = userId!, Id = id });
            if (tagDto == null)
            {
                return NotFound();
            }

            return Ok(tagDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagDto tagDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var id = await _mediator.Send(new CreateTagCommand { TagDto = tagDto, UserId = userId! });
            return Ok(new { Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] TagDto tagDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _mediator.Send(new EditTagCommand { Id = id, TagDto = tagDto, UserId = userId! });
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _mediator.Send(new DeleteTagCommand { Id = id, UserId = userId! });
            return Ok("Tag deleted successfully.");
        }
    }
}