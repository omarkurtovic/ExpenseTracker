using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Dtos;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace ExpenseTrackerWebApp.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        
        public CategoryService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public IQueryable<Category> GetCategoriesQuery(AppDbContext context)
        {
            return context.Categories.Where(t => t.IdentityUserId == _currentUserService.GetUserId());
        }
        public async Task<List<Category>> GetAllAsync()
        {
            return await GetCategoriesQuery(_context).OrderBy(t => t.Id).ToListAsync();
        }
        public async Task<List<Category>> GetAllWithTransactionsAsync()
        {
            return await GetCategoriesQuery(_context).Include(c => c.Transactions).OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<Category?> GetAsync(int categoryId)
        {
            return await GetCategoriesQuery(_context).SingleAsync(c => c.Id == categoryId);
        }

        public async Task DeleteAsync(int categoryId)
        {
            await _context.Transactions.Where(t => t.CategoryId == categoryId).ExecuteDeleteAsync();
            await GetCategoriesQuery(_context).Where(c => c.Id == categoryId).ExecuteDeleteAsync();
        }

        public async Task DeleteAllAsync()
        {
            var categoryIds = await GetCategoriesQuery(_context).Select(c => c.Id).ToListAsync();
            foreach(var categoryId in categoryIds)
            {
                await _context.Transactions.Where(t => t.CategoryId == categoryId).ExecuteDeleteAsync();
            }
            await GetCategoriesQuery(_context).ExecuteDeleteAsync();
        }

        
        public async Task SaveAsync(CategoryDto categoryDto)
        {
            if(string.IsNullOrWhiteSpace(categoryDto.Name) ||
                string.IsNullOrWhiteSpace(categoryDto.UserId) ||
                string.IsNullOrWhiteSpace(categoryDto.Icon) ||
                string.IsNullOrWhiteSpace(categoryDto.Color) ||
                categoryDto.Type == null)
                {
                    return;
                }
            var category = new Category
            {
                Id = categoryDto.Id ?? 0,
                Name = categoryDto.Name,
                Type = (TransactionType)categoryDto.Type,
                IdentityUserId = categoryDto.UserId,
                Icon = categoryDto.Icon,
                Color = categoryDto.Color
            };

            await InternalSave(category);
        }
        public async Task SaveAsync(Category category)
        {
            await InternalSave(category);
        }

        private async Task InternalSave(Category category)
        {
            if (category.Id == 0)
            {
                await _context.Categories.AddAsync(category);
            }
            else
            {
                var oldCategory = await _context.Categories.SingleAsync(t => t.Id == category.Id);
                if (oldCategory != null)
                {
                    _context.Entry(oldCategory).CurrentValues.SetValues(category);
                }
            }
            await _context.SaveChangesAsync();
        }


        public static List<Category> GetDefaultCategories()
        {
            var result = new List<Category>
            {
                new() {  Name = "Housing", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Home, Color="#FF5733" },
                new() {  Name = "Transportation", Type=TransactionType.Expense, Icon=Icons.Material.Filled.DirectionsCar, Color="#33C1FF" },
                new() {  Name = "Groceries", Type=TransactionType.Expense, Icon=Icons.Material.Filled.LocalGroceryStore, Color="#33FF57" },
                new() {  Name = "Food & Dining", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Restaurant, Color="#FF33A8" },
                new() {  Name = "Healthcare", Type=TransactionType.Expense, Icon=Icons.Material.Filled.HealthAndSafety, Color="#FF8C33" },
                new() {  Name = "Personal Care", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Person, Color="#8C33FF" },
                new() {  Name = "Family Care", Type=TransactionType.Expense, Icon=Icons.Material.Filled.FamilyRestroom, Color="#33FFDD" },
                new() {  Name = "Debt Payments", Type=TransactionType.Expense, Icon=Icons.Material.Filled.CreditCard, Color="#FF3333" },
                new() {  Name = "Entertainment", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Movie, Color="#33FF57" },
                new() {  Name = "Savings & Investing", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Savings, Color="#3357FF" },
                new() {  Name = "Technology", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Computer, Color="#FF33F6" },
                new() {  Name = "Other", Type=TransactionType.Expense, Icon=Icons.Material.Filled.Category, Color="#A9A9A9" },
                new() {  Name = "Income", Type=TransactionType.Income, Icon=Icons.Material.Filled.AttachMoney, Color="#33FF57" },
            };

            for(int i = 1; i <= result.Count; i++)
            {
                result[i - 1].Id = i;
            }
            return result;
        }

        public static List<string> GetDefaultCategoryIcons()
        {
            var defaultCategories = GetDefaultCategories();
            var result = defaultCategories.Select(c => c.Icon).ToList();
            result.Add(Icons.Material.Filled.Fastfood);
            result.Add(Icons.Material.Filled.LocalCafe);
            result.Add(Icons.Material.Filled.DirectionsTransit);
            result.Add(Icons.Material.Filled.ShoppingCart);
            result.Add(Icons.Material.Filled.Build);
            result.Add(Icons.Material.Filled.Phone);
            result.Add(Icons.Material.Filled.FitnessCenter);
            return result;
        }

        public async Task AssignUserDefaultCategories(string userId)
        {
            var defaultCategories = GetDefaultCategories();
            foreach(var category in defaultCategories)
            {
                category.Id = 0;
                category.IdentityUserId = userId;
                await SaveAsync(category);
            }
        }

    }
}