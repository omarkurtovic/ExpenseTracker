using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.Budgets.Dtos;
using ExpenseTrackerWasmWebApp.Services;

namespace ExpenseTrackerWasmWebApp.Features.Budgets.Services
{
    public class BudgetService(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Result<List<BudgetWithProgressDto>>> GetBudgetsAsync()
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var response = await http.GetAsync("api/budgets");
                if (response.IsSuccessStatusCode)
                {
                    var budgets = await response.Content.ReadFromJsonAsync<List<BudgetWithProgressDto>>() ?? new();
                    return Result<List<BudgetWithProgressDto>>.Success(budgets);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<List<BudgetWithProgressDto>>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching budgets! Status Code: {response.StatusCode}!");
                    return Result<List<BudgetWithProgressDto>>.Failure("Failed to fetch budgets.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<List<BudgetWithProgressDto>>.Failure("An error occurred while fetching budgets.");
            }
        }

        public async Task<Result> CreateBudgetAsync(BudgetDto budgetDto)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/budgets");
                request.Content = JsonContent.Create(budgetDto);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error creating budget! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to create budget.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while creating the budget.");
            }
        }

        public async Task<Result> EditBudgetAsync(BudgetDto budgetDto)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var request = new HttpRequestMessage(HttpMethod.Put, $"api/budgets/{budgetDto.Id}");
                request.Content = JsonContent.Create(budgetDto);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error updating budget! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to update budget.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while updating the budget.");
            }
        }

        public async Task<Result> DeleteBudgetAsync(int budgetId)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var response = await http.DeleteAsync($"api/budgets/{budgetId}");
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error deleting budget! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to delete budget.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while deleting the budget.");
            }
        }
    }
}
