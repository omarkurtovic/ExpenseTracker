using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTrackerSharedCL.Features.Logging.Dtos
{
    public class LogsGridOptionsDto
    {
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 0;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}
