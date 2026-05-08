using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Logging.Enums;
using ExpenseTrackerWebApi.Features.Logging.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace ExpenseTrackerWebApi.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        private readonly RequestDelegate _next = next;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (FluentValidation.ValidationException valEx)
            {
                var validationErrors = valEx.Errors
                    .Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage })
                    .ToList();

                var errorJson = JsonSerializer.Serialize(validationErrors);

                await LogEventToDatabaseAsync(context, valEx.Message, errorJson, LogType.Validation);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    ErrorMessage = "Validation failed.",
                    Errors = validationErrors
                });
            }
            catch (UnauthorizedAccessException unauthEx)
            {
                await LogEventToDatabaseAsync(context, "Unauthorized Access Attempt", unauthEx.Message, LogType.Exception);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = "You do not have permission to access this resource." });
            }
            catch (KeyNotFoundException notFoundEx)
            {
                await LogEventToDatabaseAsync(context, "Resource Not Found", notFoundEx.Message, LogType.Action);

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = "The requested resource could not be found." });
            }
            catch (Exception ex)
            {
                await LogExceptionToDatabaseAsync(context, ex);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { ErrorMessage = "An unexpected server error occurred." });
            }
        }

        private async Task LogEventToDatabaseAsync(HttpContext context, string message, string? details, LogType logType)
        {
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var log = new SystemLog
                {
                    Type = logType,
                    IdentityUserId = userId,
                    RequestName = context.Request.Path,
                    Message = message,
                    Details = details
                };

                dbContext.SystemLogs.Add(log);
                await dbContext.SaveChangesAsync();
            });
        }

        private async Task LogExceptionToDatabaseAsync(HttpContext context, Exception ex)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var log = new SystemLog
            {
                Type = LogType.Exception,
                IdentityUserId = userId,
                RequestName = context.Request.Path,
                Message = ex.Message,
                Details = ex.StackTrace
            };

            dbContext.SystemLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
