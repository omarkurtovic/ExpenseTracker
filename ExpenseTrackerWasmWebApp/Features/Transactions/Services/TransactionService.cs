using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Transactions;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;
using ExpenseTrackerWasmWebApp.Services;

namespace ExpenseTrackerWasmWebApp.Features.Transactions.Services;

public class TransactionService(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<Result<TransactionsPageDataDto>> GetTransactionsAsync(TransactionsGridOptionsDto options)
    {
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            var response = await http.PostAsJsonAsync("api/transactions/search", options);
            if (response.IsSuccessStatusCode)
            {
                var transactions = await response.Content.ReadFromJsonAsync<TransactionsPageDataDto>() ?? new();
                return Result<TransactionsPageDataDto>.Success(transactions);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result<TransactionsPageDataDto>.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error fetching transactions! Status Code: {response.StatusCode}!");
            return Result<TransactionsPageDataDto>.Failure("Failed to fetch transactions.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result<TransactionsPageDataDto>.Failure("An error occurred while fetching transactions.");
        }
    }

    public async Task<Result> DeleteTransactionAsync(int transactionId)
    {
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            var response = await http.DeleteAsync($"api/transactions/{transactionId}");
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error deleting transaction! Status Code: {response.StatusCode}!");
            return Result.Failure("Failed to delete transaction.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result.Failure("An error occurred while deleting the transaction.");
        }
    }

    public async Task<Result> CreateTransactionAsync(TransactionDto transactionDto)
    {
        transactionDto.AccountDto = null;
        transactionDto.CategoryDto = null;
        transactionDto.TransactionTags = [];
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/transactions");
            request.Content = JsonContent.Create(transactionDto);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error creating transaction! Status Code: {response.StatusCode}!");
            return Result.Failure("Failed to create transaction.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result.Failure("An error occurred while creating the transaction.");
        }
    }

    public async Task<Result> UpdateTransactionAsync(TransactionDto transactionDto)
    {
        transactionDto.AccountDto = null;
        transactionDto.CategoryDto = null;
        transactionDto.TransactionTags = [];
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/transactions");
            request.Content = JsonContent.Create(transactionDto);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Result.Failure("Unauthorized access.", FailureReason.Unauthorized);
            }

            Console.WriteLine($"Error updating transaction! Status Code: {response.StatusCode}!");
            return Result.Failure("Failed to update transaction.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Result.Failure("An error occurred while updating the transaction.");
        }
    }
}