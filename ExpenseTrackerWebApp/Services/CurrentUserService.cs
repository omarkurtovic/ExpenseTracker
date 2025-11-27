using System.Security.Claims;

namespace ExpenseTrackerWebApp.Services
{
    public interface ICurrentUserService
    {
        public string GetUserId();
        public string GetUsername();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
    
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    
        public string GetUserId()
        {
            if(_httpContextAccessor.HttpContext == null)
            {
                throw new Exception("HTTP Context is null");
            }

            string? userId = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("User is not authenticated");
            }

            return userId;
        }
    
        public string GetUsername()
        {
            if(_httpContextAccessor.HttpContext == null)
            {
                throw new Exception("HTTP Context is null");
            }

            string? username = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.Name);

            if(string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("User is not authenticated");
            }

            return username;
        }
    }
}