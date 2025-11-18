using System.Security.Claims;

namespace ExpenseTrackerWebApp.Services
{
    public interface ICurrentUserService
    {
        public string? GetUserId();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
    
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    
        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier) ;
        }
    }
}