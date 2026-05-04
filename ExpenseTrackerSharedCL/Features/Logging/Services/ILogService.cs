using ExpenseTrackerSharedCL.Features.Logging.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Logging.Services
{
    public interface ILogService
    {
        public Task<Result<LogsPageDataDto>> GetLogsAsync(LogsGridOptionsDto options);
    }
}
