using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerWasmWebApp.Services;

namespace ExpenseTrackerWasmWebApp.Features.Accounts.Services
{
    public class AccountService(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<Result<List<AccountDto>>> GetAccountsAsync()
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                string url = $"api/accounts";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await http.SendAsync(request);
                var accounts = new List<AccountDto>();
                if (response.IsSuccessStatusCode)
                {
                    accounts = await response.Content.ReadFromJsonAsync<List<AccountDto>>() ?? new();
                    return Result<List<AccountDto>>.Success(accounts);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<List<AccountDto>>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching accounts! Status Code: {response.StatusCode}!");
                    return Result<List<AccountDto>>.Failure("Failed to fetch accounts.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<List<AccountDto>>.Failure("An error occurred while fetching accounts.");
            }
        }

        public async Task<Result<List<AccountWithBalanceDto>>> GetAccountsWithBalanceAsync()
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                string url = $"api/accounts/with-balances";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await http.SendAsync(request);
                var accounts = new List<AccountWithBalanceDto>();
                if (response.IsSuccessStatusCode)
                {
                    accounts = await response.Content.ReadFromJsonAsync<List<AccountWithBalanceDto>>() ?? new();
                    return Result<List<AccountWithBalanceDto>>.Success(accounts);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<List<AccountWithBalanceDto>>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching accounts with balances! Status Code: {response.StatusCode}!");
                    return Result<List<AccountWithBalanceDto>>.Failure("Failed to fetch accounts with balances.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<List<AccountWithBalanceDto>>.Failure("An error occurred while fetching accounts with balances.");
            }
        }

        public async Task<Result<AccountDto>> GetAccountAsync(int accountId)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                string url = $"api/accounts/{accountId}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var account = await response.Content.ReadFromJsonAsync<AccountDto>();
                    return Result<AccountDto>.Success(account!);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result<AccountDto>.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error fetching account! Status Code: {response.StatusCode}!");
                    return Result<AccountDto>.Failure("Failed to fetch account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<AccountDto>.Failure("An error occurred while fetching the account.");
            }
        }


        public async Task<Result> CreateAccountAsync(AccountDto accountDto)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                string url = $"api/accounts";
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = JsonContent.Create(accountDto);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error creating account! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to create account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while creating the account.");
            }
        }
    
    
        public async Task<Result> EditAccountAsync(AccountDto accountDto)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                string url = $"api/accounts/{accountDto.Id}";
                var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Content = JsonContent.Create(accountDto);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error updating account! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to update account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while updating the account.");
            }
        }

        public async Task<Result> DeleteAccountAsync(int accountId)
        {
            try
            {
                var http = _httpClientFactory.CreateClient("WebAPI");
                string url = $"api/accounts/{accountId}";
                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return Result.Success();
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
                }
                else
                {
                    Console.WriteLine($"Error deleting account! Status Code: {response.StatusCode}!");
                    return Result.Failure("Failed to delete account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.Failure("An error occurred while deleting the account.");
            }
        }
    }
}