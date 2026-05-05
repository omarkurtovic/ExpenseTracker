using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.UserPreferences.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerWebApi.Services
{

    public class DatabaseSeeder(
        AppDbContext db,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration config)
    {
        private readonly AppDbContext _db = db;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IConfiguration _config = config;

        public async Task SeedAsync()
        {
            await _db.Database.MigrateAsync();

            if (!await _roleManager.RoleExistsAsync("Administrator"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            await AddTestUserAsync();
            await AddAdminUserAsync();
            await AddDefaultsToCategoriesIfNeededAsync();
            await AddDefaultsToAccountsIfNeededAsync();
        }

        private async Task AddTestUserAsync()
        {
            string testUsername = "sa";
            string testPassword = "Secret1!";

            var existingTestUser = await _userManager.FindByNameAsync(testUsername);
            if (existingTestUser == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = testUsername,
                    Email = "sa@mytracker.com",
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(newUser, testPassword);
                await AddUserPreferenceIfNeededAsync(newUser.Id);
            }
        }

        private async Task AddAdminUserAsync()
        {
            var adminEmail = _config["AdminSettings:Email"];
            var adminPassword = _config["AdminSettings:Password"];

            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
            {
                var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
                if (existingAdmin == null)
                {
                    var newAdmin = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(newAdmin, adminPassword);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newAdmin, "Administrator");
                        await AddUserPreferenceIfNeededAsync(newAdmin.Id);
                    }
                }
            }
        }

        private async Task AddDefaultsToAccountsIfNeededAsync()
        {
            var accountsNeedingDefaults = await _db.Accounts
                .Where(a => string.IsNullOrEmpty(a.Color) || string.IsNullOrEmpty(a.Icon))
                .ToListAsync();

            if (accountsNeedingDefaults.Count != 0)
            {
                var iconColors = new[] { "#FF5733", "#33C1FF", "#33FF57", "#FF33A8" };
                var icons = new[]
                {
                "Icons.Material.Filled.Payments",
                "Icons.Material.Filled.AccountBalance",
                "Icons.Material.Filled.Wallet",
                "Icons.Material.Filled.CreditCard"
            };

                for (int i = 0; i < accountsNeedingDefaults.Count; i++)
                {
                    var account = accountsNeedingDefaults[i];
                    account.Color ??= iconColors[i % iconColors.Length];
                    account.Icon ??= icons[i % icons.Length];
                }
                await _db.SaveChangesAsync();
            }
        }

        private async Task AddDefaultsToCategoriesIfNeededAsync()
        {
            var categoriesNeedingDefaults = await _db.Categories
                .Where(c => string.IsNullOrEmpty(c.Color) || string.IsNullOrEmpty(c.Icon))
                .ToListAsync();

            if (categoriesNeedingDefaults.Count != 0)
            {
                foreach (var category in categoriesNeedingDefaults)
                {
                    category.Color ??= "#A9A9A9";
                    category.Icon ??= "Icons.Material.Filled.Category";
                }
                await _db.SaveChangesAsync();
            }
        }

        private async Task AddUserPreferenceIfNeededAsync(string userId)
        {
            var userPreferences = await _db.UserPreferences
                .Where(us => us.UserId == userId)
                .ToListAsync();

            if (userPreferences.Count == 0)
            {
                _db.UserPreferences.Add(new UserPreference() { UserId = userId, DarkMode = false });
                await _db.SaveChangesAsync();
            }
        }
    }
}