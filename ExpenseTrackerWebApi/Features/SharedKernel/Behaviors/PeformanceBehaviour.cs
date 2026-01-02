using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace ExpenseTrackerWebApi.Features.SharedKernel.Behaviors
{

    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger; private readonly IHttpContextAccessor _httpContextAccessor;


        public PerformanceBehaviour(
            ILogger<TRequest> logger,
            IHttpContextAccessor httpContextAccessor)
            {
                _timer = new Stopwatch();

                _logger = logger;
                _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _httpContextAccessor.HttpContext?.User
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                var username = _httpContextAccessor.HttpContext?.User
                    .FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";


                _logger.LogWarning(
                   "VerticalSlice Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) UserId: {UserId} Username: {Username} {@Request}",
                   requestName,
                   elapsedMilliseconds,
                   userId,
                   username,
                   request);
            }

            return response;
        }
    }
}