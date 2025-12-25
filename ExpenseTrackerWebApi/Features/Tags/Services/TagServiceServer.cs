using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Service;
using ExpenseTrackerWebApi.Features.Tags.Queries;
using MediatR;
using System.Security.Claims;

namespace ExpenseTrackerWebApi.Features.Tags.Services
{
    public class TagServiceServer : ITagService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TagServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<Result<TagDto>> CreateTagAsync(TagDto tagDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteTagAsync(int tagId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<TagDto>>> GetTagsAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tags = await _mediator.Send(new GetTagsQuery { UserId = userId! });
                return Result<List<TagDto>>.Success(tags);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tags: {ex.Message}");
                return Result<List<TagDto>>.Failure("An error occurred while fetching tags.");
            }
        }
    }
}
