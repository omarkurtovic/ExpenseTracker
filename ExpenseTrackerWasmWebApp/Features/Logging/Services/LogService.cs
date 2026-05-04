using ExpenseTrackerSharedCL.Features.Logging.Dtos;
using ExpenseTrackerSharedCL.Features.Logging.Services;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerSharedCL.Features.Transactions.Services;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Features.Logging.Services
{
    public class LogService(HttpClient httpClient) : ILogService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<LogsPageDataDto>> GetLogsAsync(LogsGridOptionsDto options)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/logs/search", options);
                if (response.IsSuccessStatusCode)
                {
                    var logs = await response.Content.ReadFromJsonAsync<LogsPageDataDto>() ?? new();
                    return Result<LogsPageDataDto>.Success(logs);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<LogsPageDataDto>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }

                Console.WriteLine($"Error fetching logs! Status Code: {response.StatusCode}!");
                return Result<LogsPageDataDto>.Failure("Failed to fetch logs.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<LogsPageDataDto>.Failure("An error occurred while fetching logs.");
            }
        }
    }
}
