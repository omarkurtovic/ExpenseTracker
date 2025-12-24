using ExpenseTrackerSharedCL.Features.Dashboard;
using ExpenseTrackerSharedCL.Features.Dashboard.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Features.Dashboard.Services
{
    public class DashboardService(HttpClient httpClient) : IDashboardService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(int monthsBehindToConsider = 5, int maxCategoriesToShow = 6)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/dashboard");
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