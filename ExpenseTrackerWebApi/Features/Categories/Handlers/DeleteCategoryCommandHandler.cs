using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Categories.Handlers
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly AppDbContext _context;

        public DeleteCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Where(a => a.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (category == null)
            {
                throw new ArgumentException("Category not found!");
            }

            if (category.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Category does not belong to user!");
            }

            await _context.Transactions.Where(t => t.CategoryId == request.Id).ExecuteDeleteAsync(cancellationToken);
            await _context.Categories.Where(c => c.Id == request.Id).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
