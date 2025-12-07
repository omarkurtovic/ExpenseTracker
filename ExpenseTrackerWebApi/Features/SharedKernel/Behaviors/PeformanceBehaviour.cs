using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTrackerWebApi.Features.SharedKernel.Behaviors
{

    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public PerformanceBehaviour(
            ILogger<TRequest> logger,
            UserManager<IdentityUser> userManager)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _userManager = userManager;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                // todo ovo popraviti
                //var user = await _userManager.GetUserAsync(User);
                var requestName = typeof(TRequest).Name;
                //var userId = user?.Id ?? string.Empty;

                _logger.LogWarning(
                    "VerticalSlice Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Request}",
                    requestName,
                    elapsedMilliseconds,
                    "userId",
                    request);
            }

            return response;
        }
    }
}