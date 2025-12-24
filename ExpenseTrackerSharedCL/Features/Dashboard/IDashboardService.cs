using ExpenseTrackerSharedCL.Features.Dashboard.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Dashboard
{
    public interface IDashboardService
    {
        public Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(int monthsBehindToConsider = 5, int maxCategoriesToShow = 6);
    }
}
