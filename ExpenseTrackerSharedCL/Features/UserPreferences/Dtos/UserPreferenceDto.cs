namespace ExpenseTrackerSharedCL.Features.UserPreferences.Dtos
{
    public class UserPreferenceDto
    {
        public string UserId{get; set;} = string.Empty;
        public bool DarkMode{get; set;}
    }
}