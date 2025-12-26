using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.DataSeeding.Dtos;
using ExpenseTrackerSharedCL.Features.SharedKernel;

namespace ExpenseTrackerWasmWebApp.Features.DataSeeding.Services
{
    public class DataSeedService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result> SeedDataAsync(DataSeedOptionsDto dataSeedOptionsDto)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"api/dataSeeding");
                request.Content = JsonContent.Create(dataSeedOptionsDto);
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
                    Console.WriteLine($"Error seeding data! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to seed data.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while seeding data.");
            }
        }
        
    }
}