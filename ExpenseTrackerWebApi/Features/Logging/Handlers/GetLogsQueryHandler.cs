using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Logging.Dtos;
using ExpenseTrackerSharedCL.Features.Logging.Enums;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Enums;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Logging.Queries;
using ExpenseTrackerWebApi.Features.Transactions.Models;
using ExpenseTrackerWebApi.Features.Transactions.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;
using System.Globalization;

namespace ExpenseTrackerWebApi.Features.Logging.Handlers
{
    public class GetLogsQueryHandler : IRequestHandler<GetLogsPageDataQuery, LogsPageDataDto>
    {
        private readonly AppDbContext _context;

        public GetLogsQueryHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<LogsPageDataDto> Handle(GetLogsPageDataQuery request, CancellationToken cancellationToken)
        {
            var logQuery = _context.SystemLogs.Include(sl => sl.IdentityUser).Where(_ => 1 == 1);
            var options = request.LogOptions;

            if (!string.IsNullOrWhiteSpace(options.SortBy))
            {
                switch (options.SortBy)
                {
                    case nameof(SystemLogDto.Username):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.IdentityUser.UserName) : logQuery.OrderBy(t => t.IdentityUser.UserName);
                        break;
                    case nameof(SystemLogDto.Timestamp):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.Timestamp) : logQuery.OrderBy(t => t.Timestamp);
                        break;
                    case nameof(SystemLogDto.Type):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.Type) : logQuery.OrderBy(t => t.Type);
                        break;
                    case nameof(SystemLogDto.RequestName):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.RequestName) : logQuery.OrderBy(t => t.RequestName);
                        break;
                    case nameof(SystemLogDto.Message):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.Message) : logQuery.OrderBy(t => t.Message);
                        break;
                    case nameof(SystemLogDto.Details):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.Details) : logQuery.OrderBy(t => t.Details);
                        break;
                    case nameof(SystemLogDto.ElapsedMilliseconds):
                        logQuery = options.SortDescending ? logQuery.OrderByDescending(t => t.ElapsedMilliseconds) : logQuery.OrderBy(t => t.ElapsedMilliseconds);
                        break;
                    default:
                        logQuery = logQuery.OrderByDescending(t => t.Timestamp);
                        break;
                }
            }
            else
            {
                logQuery = logQuery.OrderByDescending(t => t.Timestamp);
            }

            int totalItems = await logQuery.CountAsync(cancellationToken: cancellationToken);
            logQuery = logQuery.Skip(options.CurrentPage * options.PageSize).Take(options.PageSize);

            var logs = (await logQuery.ToListAsync(cancellationToken: cancellationToken)).Select(log => new SystemLogDto()
            {
                Username = log.IdentityUser.UserName ?? string.Empty,
                Timestamp = log.Timestamp,
                Type = (LogTypeDto)log.Type,
                RequestName = log.RequestName,
                Message = log.Message,
                Details = log.Details,
                ElapsedMilliseconds = log.ElapsedMilliseconds
            }).ToList();

            return new LogsPageDataDto
            {
                Logs = logs,
                TotalItems = totalItems
            };
        }
    }
}