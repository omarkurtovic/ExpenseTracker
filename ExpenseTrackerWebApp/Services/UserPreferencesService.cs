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


        private IQueryable<UserPreference> GetPreferencesQuery(AppDbContext context)
        {
            return context.UserPreferences.Where(a => a.UserId == _currentUserService.GetUserId());
        }
        public async Task<UserPreference?> GetAsync()
        {
            return await GetPreferencesQuery(_context).SingleOrDefaultAsync();
        }

        public async Task SaveAsync(bool darkMode)
        {
            var preferences = await GetPreferencesQuery(_context).SingleAsync();
            preferences.DarkMode = darkMode;
            await _context.SaveChangesAsync();
        }

    }



    
}