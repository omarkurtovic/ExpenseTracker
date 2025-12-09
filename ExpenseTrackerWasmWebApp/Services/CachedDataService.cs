using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Services;

public class CachedDataService
{
    private readonly HttpClient _http;
    private List<AccountDto>? _accounts;
    private List<CategoryDto>? _categories;
    private List<TagDto>? _tags;
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
            var tagsTask = _http.GetFromJsonAsync<List<TagDto>>("api/tags");

            await Task.WhenAll(accountsTask, categoriesTask);

            _accounts = await accountsTask ?? new();
            _categories = await categoriesTask ?? new();
            _tags = await tagsTask ?? new();
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
    public List<TagDto> GetTags()
    {
        return _tags ?? new();
    }
    
    public async Task RefreshAccountsAsync()
    {
        try
        {
            _accounts = await _http.GetFromJsonAsync<List<AccountDto>>("api/accounts") ?? new();
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
        }
        catch
        {
        }
    }
    public async Task RefreshTagsAsync()
    {
        try
        {
            _tags = await _http.GetFromJsonAsync<List<TagDto>>("api/tags") ?? new();
        }
        catch
        {
        }
    }

    public async Task<List<AccountWithBalanceDto>> GetAccountsWithBalanceAsync()
    {
        var result = new List<AccountWithBalanceDto>();
        try
        {
            result = await _http.GetFromJsonAsync<List<AccountWithBalanceDto>>("api/accounts/with-balances") ?? new();
            _accounts = [.. result.Select(r => new AccountDto
            {
                Id = r.Id,
                Name = r.Name,
                InitialBalance = r.InitialBalance,
                Color = r.Color,
                Icon = r.Icon
            })];
        }
        catch
        {
        }
        return result;
    }

    public async Task<List<CategoryWithStatsDto>> GetCategoriesWithStatsAsync(TransactionTypeDto transactionTtype)
    {
        var result = new List<CategoryWithStatsDto>();
        try
        {
            result = await _http.GetFromJsonAsync<List<CategoryWithStatsDto>>($"api/categories/with-stats?type={(int)transactionTtype}") ?? new();
            _categories = [.. result.Select(r => new CategoryDto
            {
                Id = r.Id,
                Name = r.Name,
                Type = r.Type,
                Color = r.Color,
                Icon = r.Icon
            })];
        }
        catch
        {
        }
        return result;
    }

    public void ClearCache()
    {
        _accounts = null;
        _categories = null;
        _isInitialized = false;
    }

    public bool IsInitialized => _isInitialized;
}
