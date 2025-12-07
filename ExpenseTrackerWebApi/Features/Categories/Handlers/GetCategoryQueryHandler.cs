using ExpenseTrackerWebApi.Database;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Features.Categories.Handlers
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
    {
        private readonly AppDbContext _context;

        public GetCategoryQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Where(b => b.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (category == null)
            {
                throw new ArgumentException("Category not found!");
            }

            if (category.IdentityUserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Category does not belong to user!");
            }

            return new CategoryDto
            {
                Name = category.Name,
                Type = (ExpenseTrackerSharedCL.Features.Categories.Dtos.TransactionType)category.Type,
                Icon = category.Icon,
                Color = category.Color
            };
        }
    }
}
