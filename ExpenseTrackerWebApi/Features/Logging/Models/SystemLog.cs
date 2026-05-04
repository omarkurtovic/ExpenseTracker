using ExpenseTrackerWebApi.Features.Logging.Enums;

namespace ExpenseTrackerWebApi.Features.Logging.Models
{
    public class SystemLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? UserId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public LogType Type { get; set; }

        public string RequestName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }

        public long? ElapsedMilliseconds { get; set; }
    }

}
