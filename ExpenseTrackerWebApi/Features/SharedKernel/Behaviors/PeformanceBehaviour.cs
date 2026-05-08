using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Logging.Enums;
using ExpenseTrackerWebApi.Features.Logging.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace ExpenseTrackerWebApi.Features.SharedKernel.Behaviors
{

    public class PerformanceBehaviour<TRequest, TResponse>(
        IServiceScopeFactory scopeFactory,
        IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer = new(); 
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor; 
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next(cancellationToken);

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _httpContextAccessor.HttpContext?.User
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                var username = _httpContextAccessor.HttpContext?.User
                    .FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";

                _ = Task.Run(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();

                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var log = new SystemLog
                    {
                        Type = LogType.Performance,
                        IdentityUserId = userId,
                        RequestName = requestName,
                        ElapsedMilliseconds = elapsedMilliseconds,
                        Message = $"Long Running Request Detected: {requestName} ({elapsedMilliseconds} milliseconds)"
                    };

                    dbContext.SystemLogs.Add(log);
                    await dbContext.SaveChangesAsync();
                }, cancellationToken);
            }

            return response;
        }
    }
}