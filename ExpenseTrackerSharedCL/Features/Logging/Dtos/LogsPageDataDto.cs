using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Logging.Dtos
{
    public class LogsPageDataDto
    {
        public List<SystemLogDto> Logs { get; set; } = [];
        public int TotalItems { get; set; }
    }
}
