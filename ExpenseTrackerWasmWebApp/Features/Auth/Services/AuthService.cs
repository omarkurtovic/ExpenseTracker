using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.Auth.Dtos;
using ExpenseTrackerWasmWebApp.Services;

namespace ExpenseTrackerWasmWebApp.Features.Auth.Services
{
    public class AuthService(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                
                var http = _httpClientFactory.CreateClient("WebAPI");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/login");
                request.Content = JsonContent.Create(loginRequest);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    return Result<LoginResponseDto>.Success(loginResponse!);
                }
                else
                {
                    Console.WriteLine($"Error logging in! Status Code: {response.StatusCode}!");
                    return Result<LoginResponseDto>.Failure("Invalid username or password.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<LoginResponseDto>.Failure("An error occurred while logging in.");
            }
        }

        public async Task<Result> RegisterAsync(RegisterDataDto registerRequest)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/register");
                request.Content = JsonContent.Create(registerRequest);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else
                {
                    Console.WriteLine($"Error registering! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to register user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while registering.");
            }
        }
    }
}