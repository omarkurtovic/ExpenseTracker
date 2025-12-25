using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Budgets.Services
{
    public interface IBudgetService
    {
        public Task<Result<List<BudgetWithProgressDto>>> GetBudgetsAsync();

        public Task<Result> CreateBudgetAsync(BudgetDto budgetDto);

        public Task<Result> EditBudgetAsync(BudgetDto budgetDto);

        public Task<Result> DeleteBudgetAsync(int budgetId);
    }
}
