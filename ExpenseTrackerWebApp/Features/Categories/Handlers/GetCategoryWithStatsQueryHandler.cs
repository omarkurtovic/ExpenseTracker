using ExpenseTrackerWebApp.Database;
using MediatR;
using ExpenseTrackerWebApp.Features.Categories.Queries;
using ExpenseTrackerWebApp.Features.Categories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.Categories.Handlers
{
    public class GetCategoriesWithStatsQueryHandler : IRequestHandler<GetCategoriesWithStatsQuery, List<CategoryWithStatsDto>>
    {
        private readonly AppDbContext _context;

        public GetCategoriesWithStatsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryWithStatsDto>> Handle(GetCategoriesWithStatsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories.Where(c => c.IdentityUserId == request.UserId)
            .Where(c => request.Type == null || c.Type == request.Type)
            .Include(c => c.Transactions)
            .Select( c => new CategoryWithStatsDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                TransactionsCount = c.Transactions.Count,
                TotalAmount = c.Transactions.Sum(t => t.Amount),
                CurrentMonthAmount = c.Transactions.Where(t => t.Date.Month == DateTime.Now.Month && t.Date.Year == DateTime.Now.Year).Sum(t => t.Amount),
                LastMonthAmount = c.Transactions.Where(t => t.Date.Month == DateTime.Now.AddMonths(-1).Month && t.Date.Year == DateTime.Now.AddMonths(-1).Year).Sum(t => t.Amount),
                Icon = c.Icon,
                Color = c.Color
            }).ToListAsync(cancellationToken);
        }
    }
}