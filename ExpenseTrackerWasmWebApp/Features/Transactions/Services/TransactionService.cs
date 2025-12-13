using System.Net.Http.Json;
using ExpenseTrackerSharedCL.Features.Transactions.Dtos;

namespace ExpenseTrackerWasmWebApp.Features.Transactions.Services
{
    public class TransactionService
    {
        private readonly HttpClient _http;
        public TransactionService(HttpClient http)
        {
            _http = http;
        }
        public async Task<TransactionsPageDataDto> GetTransactions(TransactionsGridOptionsDto options)
        {
            var result = new TransactionsPageDataDto();
            try
            {
                string url = $"api/transactions/search";
                var response = await _http.PostAsJsonAsync(url, options);
                if(response.IsSuccessStatusCode)
                {
                    var transactions = await response.Content.ReadFromJsonAsync<TransactionsPageDataDto>() ?? new();
                    return transactions;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public async Task<bool> DeleteTransaction(int transactionId)
        {
            try
            {
                string url = $"api/transactions/{transactionId}";
                var request = new HttpRequestMessage(HttpMethod.Delete, url);
                var response = await _http.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


    }
}