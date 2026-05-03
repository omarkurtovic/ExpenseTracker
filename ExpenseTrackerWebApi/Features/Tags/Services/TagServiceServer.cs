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

        public Task<Result<TagDto>> EditTagAsync(TagDto tagDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<TagDto>>> GetTagsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
