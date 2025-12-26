using ExpenseTrackerSharedCL.Features.SharedKernel;
using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using ExpenseTrackerSharedCL.Features.UserPreferences.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Features.UserPreferences.Services
{
    public class UserPreferenceService(HttpClient httpClient) : IUserPreferenceService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<UserPreferenceDto>> GetUserPreferencesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/userpreferences");
                if (response.IsSuccessStatusCode)
                {
                    var preferences = await response.Content.ReadFromJsonAsync<UserPreferenceDto>() ?? new();
                    return Result<UserPreferenceDto>.Success(preferences);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<UserPreferenceDto>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching user preferences! Status Code: {response.StatusCode}!");
                    return Result<UserPreferenceDto>.Failure("Failed to fetch user preferences.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<UserPreferenceDto>.Failure("An error occurred while fetching user preferences.");
            }
        }

        public async Task<Result> UpdateUserPreferencesAsync(UserPreferenceDto preferenceDto)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, $"api/userpreferences");
                request.Content = JsonContent.Create(preferenceDto);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(request);
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
                    Console.WriteLine($"Error updating user preferences! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to update user preferences.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while updating user preferences.");
            }
        }

        public async Task<Result> CreateDefaultUsrPreferencesAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"api/userpreferences/default");
                var response = await _httpClient.SendAsync(request);
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
                    Console.WriteLine($"Error setting default user preferences! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to set default user preferences.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while setting default user preferences.");
            }
        }
    }
}