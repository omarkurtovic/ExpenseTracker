using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Features.SharedKernel.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Handlers
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<Tag>>
    {
        private readonly AppDbContext _context;

        public GetTagsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Tag>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Tags
            .Where(t => t.IdentityUserId == request.UserId)
            .ToListAsync();
        }
    }
}