using ExpenseTrackerWebApi.Database;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerWebApi.Features.Tags.Queries;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;

namespace ExpenseTrackerWebApi.Features.Tags.Handlers
{
    public class GetTagQueryHandler : IRequestHandler<GetTagQuery, TagDto>
    {
        private readonly AppDbContext _context;

        public GetTagQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TagDto> Handle(GetTagQuery request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags
                .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                throw new ArgumentException("Tag not found!");
            }

            if (tag.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Tag does not belong to user!");
            }

            return new TagDto()
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color
            };
        }
    }
}
