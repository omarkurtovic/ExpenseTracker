using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApi.Database.Models
{
    public class UserPreference{
        public string UserId{get; set;} = string.Empty;
        public IdentityUser IdentityUser{get; set;} = null!;
        public bool DarkMode{get; set;}
    }
}