using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Features.SharedKernel.Queries
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<Category>>
    {
        private readonly AppDbContext _context;

        public GetCategoriesQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Where(c => c.IdentityUserId == request.UserId)
                .ToListAsync();
        }
    }
}