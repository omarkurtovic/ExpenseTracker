using Database;
using ExpenseTracker.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class CategoryService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        
        public CategoryService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Categories.OrderBy(t => t.Id).ToListAsync();
        }

    }
}