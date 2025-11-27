using ExpenseTrackerWebApp.Services;

namespace ExpenseTrackerTests.Services
{
    public class TestCurrentUserService : ICurrentUserService
    {
        public string CurrentUserId{get; set;}
        public string CurrentUserEmail{get; set;}
        public TestCurrentUserService(string userId, string userEmail = "")
        {
            CurrentUserId = userId;
            CurrentUserEmail = userEmail;
        }
        public string GetUserId()
        {
            return CurrentUserId;
        }
        public string GetUsername()
        {
            return CurrentUserEmail;
        }
    }
}