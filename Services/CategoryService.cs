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

        public async Task<Category?> GetAsync(int categoryId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Categories.SingleAsync(c => c.Id == categoryId);
        }

        public async Task DeleteAsync(int categoryId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Transactions.Where(t => t.CategoryId == categoryId).ExecuteDeleteAsync();
            await context.Categories.Where(c => c.Id == categoryId).ExecuteDeleteAsync();
        }

        
        public async Task SaveAsync(Category category)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (category.Id == 0)
            {
                await context.Categories.AddAsync(category);
            }
            else
            {
                var oldCategory = await context.Categories.SingleAsync(t => t.Id == category.Id);
                if (oldCategory != null)
                {
                    context.Entry(oldCategory).CurrentValues.SetValues(category);
                }
            }
            await context.SaveChangesAsync();
        }

    }
}