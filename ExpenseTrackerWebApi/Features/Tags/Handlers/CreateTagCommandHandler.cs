using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using ExpenseTrackerWebApi.Features.Tags.Commands;
using ExpenseTrackerWebApi.Features.Tags.Models;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Tags.Handlers
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateTagCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var tag = new Tag()
            {
                Name = request.TagDto.Name!,
                Color = request.TagDto.Color,
                IdentityUserId = request.UserId
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync(cancellationToken);
            return tag.Id;
        }
    }
}
