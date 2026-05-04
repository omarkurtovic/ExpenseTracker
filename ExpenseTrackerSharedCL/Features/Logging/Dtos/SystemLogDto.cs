using ExpenseTrackerSharedCL.Features.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Logging.Dtos
{
    public class SystemLogDto
    {
        public string Username { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public LogTypeDto Type { get; set; }

        public string RequestName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }

        public long? ElapsedMilliseconds { get; set; }
    }
}
