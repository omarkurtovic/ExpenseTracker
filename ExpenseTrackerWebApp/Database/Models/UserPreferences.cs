using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Database.Models
{
    public class UserPreferences{
        public string UserId{get; set;}
        public IdentityUser IdentityUser{get; set;}
        public bool DarkMode{get; set;}
    }
}