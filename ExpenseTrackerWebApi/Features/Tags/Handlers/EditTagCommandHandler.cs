using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerWebApi.Features.Tags.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Tags.Handlers
{
    public class EditTagCommandHandler : IRequestHandler<EditTagCommand, int>
    {
        private readonly AppDbContext _context;

        public EditTagCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(EditTagCommand request, CancellationToken cancellationToken)
        {
            var oldTag = await _context.Tags
                .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (oldTag == null)
            {
                throw new ArgumentException("Tag not found!");
            }

            if (oldTag.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Tag does not belong to user!");
            }

            oldTag.Name = request.TagDto.Name!;
            oldTag.Color = request.TagDto.Color;
            await _context.SaveChangesAsync(cancellationToken);

            return oldTag.Id;
        }
    }
}
