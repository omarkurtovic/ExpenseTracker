using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApp.Database.Models
{
    public class UserPreference{
        public string UserId{get; set;}
        public IdentityUser IdentityUser{get; set;}
        public bool DarkMode{get; set;}
    }
}