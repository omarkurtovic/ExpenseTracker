namespace ExpenseTracker.Services
{
    public class TestCurrentUserService : ICurrentUserService
    {
        public string CurrentUserId{get; set;}
        public TestCurrentUserService(string userId)
        {
            CurrentUserId = userId;
        }
        public string? GetUserId()
        {
            return CurrentUserId;
        }
    }
}