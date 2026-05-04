using ExpenseTrackerWebApi.Features.Logging.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApi.Features.Logging.Models
{
    public class SystemLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string IdentityUserId { get; set; } = string.Empty;

        public IdentityUser IdentityUser { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public LogType Type { get; set; }

        public string RequestName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }

        public long? ElapsedMilliseconds { get; set; }
    }
}
