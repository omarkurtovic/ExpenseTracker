using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.Dashboard.Dtos;
using ExpenseTrackerWasmWebApp.Services;

namespace ExpenseTrackerWasmWebApp.Features.Dashboard.Services
{
    public class DashboardService(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync()
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var response = await http.GetAsync("api/dashboard");
                if (response.IsSuccessStatusCode)
                {
                    var stats = await response.Content.ReadFromJsonAsync<DashboardSummaryDto>() ?? new DashboardSummaryDto();
                    return Result<DashboardSummaryDto>.Success(stats);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<DashboardSummaryDto>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching dashboard stats! Status Code: {response.StatusCode}!");
                    return Result<DashboardSummaryDto>.Failure("Failed to fetch dashboard stats.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<DashboardSummaryDto>.Failure("An error occurred while fetching dashboard stats.");
            }
        }
        
    }
}