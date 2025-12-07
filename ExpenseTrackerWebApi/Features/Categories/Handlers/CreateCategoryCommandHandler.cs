using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Database.Models;
using ExpenseTrackerWebApi.Features.Categories.Commands;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Categories.Handlers
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category()
            {
                Name = request.CategoryDto.Name!,
                Type = (TransactionType)request.CategoryDto.Type!,
                Icon = request.CategoryDto.Icon,
                Color = request.CategoryDto.Color,
                IdentityUserId = request.UserId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Id;
        }
    }
}
