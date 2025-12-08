using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Services;

public class CachedDataService
{
    private readonly HttpClient _http;
    private List<AccountDto>? _accounts;
    private List<CategoryDto>? _categories;
    private List<AccountWithBalanceDto>? _accountsWithBalance;
    private List<CategoryWithStatsDto>? _categoriesWithStats;
    private bool _isInitialized = false;

    public CachedDataService(HttpClient http)
    {
        _http = http;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var accountsTask = _http.GetFromJsonAsync<List<AccountDto>>("api/accounts");
            var categoriesTask = _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");

            await Task.WhenAll(accountsTask, categoriesTask);

            _accounts = await accountsTask ?? new();
            _categories = await categoriesTask ?? new();
            _isInitialized = true;
        }
        catch
        {
            _accounts = new();
            _categories = new();
        }
    }

    public List<AccountDto> GetAccounts()
    {
        return _accounts ?? new();
    }

    public List<CategoryDto> GetCategories()
    {
        return _categories ?? new();
    }
    public async Task RefreshAccountsAsync()
    {
        try
        {
            _accounts = await _http.GetFromJsonAsync<List<AccountDto>>("api/accounts") ?? new();
            _accountsWithBalance = null;
        }
        catch
        {
        }
    }

    public async Task RefreshCategoriesAsync()
    {
        try
        {
            _categories = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories") ?? new();
            _categoriesWithStats = null;
        }
        catch
        {
        }
    }

    public void RemoveAccount(int accountId)
    {
        if (_accounts != null)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == accountId);
            if (account != null)
            {
                _accounts.Remove(account);
            }
        }
    }
    public void AddAccount(AccountDto account)
    {
        if (_accounts != null)
        {
            _accounts.Add(account);
        }
    }

    public void UpdateAccount(AccountDto updatedAccount)
    {
        if (_accounts != null)
        {
            var index = _accounts.FindIndex(a => a.Id == updatedAccount.Id);
            if (index != -1)
            {
                _accounts[index] = updatedAccount;
            }
        }
    }
    public void ClearCache()
    {
        _accounts = null;
        _categories = null;
        _accountsWithBalance = null;
        _categoriesWithStats = null;
        _isInitialized = false;
    }

    public bool IsInitialized => _isInitialized;
}
