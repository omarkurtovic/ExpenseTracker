using Database;
using ExpenseTracker.Database.Models;
using ExpenseTracker.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class UserPreferencesService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly CurrentUserService _currentUserService;
        
        public UserPreferencesService(IDbContextFactory<AppDbContext> contextFactory, CurrentUserService currentUserService)
        {
            _contextFactory = contextFactory;
            _currentUserService = currentUserService;
        }


        private IQueryable<UserPreferences> GetPreferencesQuery(AppDbContext context)
        {
            return context.UserPreferences.Where(a => a.UserId == _currentUserService.GetUserId());
        }
        public async Task<UserPreferences?> GetAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await GetPreferencesQuery(context).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(bool darkMode)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var preferences = await GetPreferencesQuery(context).SingleAsync();
            preferences.DarkMode = darkMode;
            await context.SaveChangesAsync();
        }
        public async Task AssignUserDefaultPreferences(string? userId = "")
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = _currentUserService.GetUserId();
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var preference = new UserPreferences
            {
                UserId = userId,
                DarkMode = false
            };

            using var context = await _contextFactory.CreateDbContextAsync();
            context.UserPreferences.Add(preference);
            await context.SaveChangesAsync();
        }

    }
}