using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerWasmWebApp.Services;

namespace ExpenseTrackerWasmWebApp.Features.Categories.Services
{
    public class CategoryService(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Result<List<CategoryDto>>> GetCategoriesAsync()
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var response = await http.GetAsync("api/categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
                    return Result<List<CategoryDto>>.Success(categories);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<List<CategoryDto>>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching categories! Status Code: {response.StatusCode}!");
                    return Result<List<CategoryDto>>.Failure("Failed to fetch categories.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<List<CategoryDto>>.Failure("An error occurred while fetching categories.");
            }
        }

        public async Task<Result<List<CategoryWithStatsDto>>> GetCategoriesWithStatsAsync()
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var response = await http.GetAsync("api/categories/with-stats");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryWithStatsDto>>() ?? new();
                    return Result<List<CategoryWithStatsDto>>.Success(categories);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<List<CategoryWithStatsDto>>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching categories with stats! Status Code: {response.StatusCode}!");
                    return Result<List<CategoryWithStatsDto>>.Failure("Failed to fetch categories with stats.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<List<CategoryWithStatsDto>>.Failure("An error occurred while fetching categories with stats.");
            }
        }

        public async Task<Result> CreateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/categories");
                request.Content = JsonContent.Create(categoryDto);
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
                    Console.WriteLine($"Error creating category! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to create category.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while creating the category.");
            }
        }

        public async Task<Result> EditCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var request = new HttpRequestMessage(HttpMethod.Put, $"api/categories/{categoryDto.Id}");
                request.Content = JsonContent.Create(categoryDto);
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
                    Console.WriteLine($"Error updating category! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to update category.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while updating the category.");
            }
        }

        public async Task<Result> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var response = await http.DeleteAsync($"api/categories/{categoryId}");
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
                    Console.WriteLine($"Error deleting category! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to delete category.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while deleting the category.");
            }
        }
    }
}
