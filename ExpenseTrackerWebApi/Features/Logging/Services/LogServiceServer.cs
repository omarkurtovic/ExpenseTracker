using ExpenseTrackerSharedCL.Features.Logging.Dtos;
using ExpenseTrackerSharedCL.Features.Logging.Services;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using MediatR;

namespace ExpenseTrackerWebApi.Features.Logging.Services
{
    public class LogServiceServer : ILogService
    {
        private readonly ISender _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogServiceServer(ISender mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<Result<LogsPageDataDto>> GetLogsAsync(LogsGridOptionsDto options)
        {
            throw new NotImplementedException();
        }
    }
}
