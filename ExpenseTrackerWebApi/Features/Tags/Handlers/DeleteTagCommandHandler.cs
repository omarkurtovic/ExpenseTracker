using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerWebApi.Features.Tags.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Tags.Handlers
{
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
    {
        private readonly AppDbContext _context;

        public DeleteTagCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _context.Tags
                .Where(a => a.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                throw new ArgumentException("Tag not found!");
            }

            if (tag.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Tag does not belong to user!");
            }

            await _context.TransactionTags.Where(tt => tt.TagId == request.Id).ExecuteDeleteAsync(cancellationToken);
            await _context.Tags.Where(c => c.Id == request.Id).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
