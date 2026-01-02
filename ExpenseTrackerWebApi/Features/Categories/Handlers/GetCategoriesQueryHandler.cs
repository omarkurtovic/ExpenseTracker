using ExpenseTrackerWebApi.Database;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DbTransactionType = ExpenseTrackerWebApi.Features.Transactions.Models.TransactionType;

namespace ExpenseTrackerWebApi.Features.Categories.Handlers
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly AppDbContext _context;

        public GetCategoriesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Where(c => c.IdentityUserId == request.UserId)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type == DbTransactionType.Income ? TransactionTypeDto.Income : TransactionTypeDto.Expense,
                    Icon = c.Icon,
                    Color = c.Color
                })
                .ToListAsync(cancellationToken);
        }
    }
}
