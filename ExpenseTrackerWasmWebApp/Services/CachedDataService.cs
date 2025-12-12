using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Services;

public class CachedDataService
{
    private List<AccountDto>? _accounts;
    private List<CategoryDto>? _categories;
    private List<TagDto>? _tags;
    private bool _isInitialized = false;
    private readonly IHttpClientFactory _httpClientFactory;

    public CachedDataService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");

            if(_accounts == null || _accounts.Count == 0)
            {
                var accounts = await http.GetFromJsonAsync<List<AccountDto>>("api/accounts");
                _accounts = accounts ?? new();
            }
            if(_categories == null || _categories.Count == 0)
            {
                var categories = await http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
                _categories = categories ?? new();
            }
            if(_tags == null || _tags.Count == 0)
            {
                var tags = await http.GetFromJsonAsync<List<TagDto>>("api/tags");
                _tags = tags ?? new();
            }
            _isInitialized = true;
        }
        catch
        {
            _accounts = new();
            _categories = new();
            _tags = new();
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
            var http = _httpClientFactory.CreateClient("WebAPI");
            _accounts = await http.GetFromJsonAsync<List<AccountDto>>("api/accounts") ?? new();
        }
        catch
        {
        }
    }

    public async Task RefreshCategoriesAsync()
    {
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            _categories = await http.GetFromJsonAsync<List<CategoryDto>>("api/categories") ?? new();
        }
        catch
        {
        }
    }
    public async Task RefreshTagsAsync()
    {
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            _tags = await http.GetFromJsonAsync<List<TagDto>>("api/tags") ?? new();
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
            var http = _httpClientFactory.CreateClient("WebAPI");
            result = await http.GetFromJsonAsync<List<AccountWithBalanceDto>>("api/accounts/with-balances") ?? new();
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

    public async Task<List<CategoryWithStatsDto>> GetCategoriesWithStatsAsync()
    {
        var result = new List<CategoryWithStatsDto>();
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            result = await http.GetFromJsonAsync<List<CategoryWithStatsDto>>($"api/categories/with-stats") ?? new();
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
