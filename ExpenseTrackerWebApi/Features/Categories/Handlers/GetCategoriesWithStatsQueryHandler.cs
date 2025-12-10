using ExpenseTrackerWebApi.Database;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerWebApi.Features.Categories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DbTransactionType = ExpenseTrackerWebApi.Database.Models.TransactionType;

namespace ExpenseTrackerWebApi.Features.Categories.Handlers
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
            return await _context.Categories
                .Where(c => c.IdentityUserId == request.UserId)
                .Include(c => c.Transactions)
                .Select(category => new
                {
                    category,
                    Transactions = category.Transactions.Where(t => t.IsReoccuring == null || t.IsReoccuring == false)
                })
                .Select(c => new CategoryWithStatsDto
                {
                    Id = c.category.Id,
                    Name = c.category.Name,
                    Type = (TransactionTypeDto)c.category.Type,
                    TransactionsCount = c.Transactions.Count(),
                    TotalAmount = c.Transactions.Sum(t => t.Amount),
                    CurrentMonthAmount = c.Transactions.Where(t => t.Date.Month == DateTime.Now.Month && t.Date.Year == DateTime.Now.Year).Sum(t => t.Amount),
                    LastMonthAmount = c.Transactions.Where(t => t.Date.Month == DateTime.Now.AddMonths(-1).Month && t.Date.Year == DateTime.Now.AddMonths(-1).Year).Sum(t => t.Amount),
                    Icon = c.category.Icon,
                    Color = c.category.Color
                }).ToListAsync(cancellationToken);
        }
    }
}
