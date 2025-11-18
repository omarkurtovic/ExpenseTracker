using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Database.Models;
using ExpenseTrackerWebApp.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApp.Services
{
    public class UserPreferencesService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        
        public UserPreferencesService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }


        private IQueryable<UserPreferences> GetPreferencesQuery(AppDbContext context)
        {
            return context.UserPreferences.Where(a => a.UserId == _currentUserService.GetUserId());
        }
        public async Task<UserPreferences?> GetAsync()
        {
            return await GetPreferencesQuery(_context).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(bool darkMode)
        {
            var preferences = await GetPreferencesQuery(_context).SingleAsync();
            preferences.DarkMode = darkMode;
            await _context.SaveChangesAsync();
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

            _context.UserPreferences.Add(preference);
            await _context.SaveChangesAsync();
        }

    }
}