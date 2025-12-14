using ExpenseTrackerSharedCL.Features.Accounts.Dtos;
using ExpenseTrackerSharedCL.Features.Categories.Dtos;
using ExpenseTrackerSharedCL.Features.Tags.Dtos;
using ExpenseTrackerSharedCL.Features.UserPreferences.Dtos;
using System.Net.Http.Json;

namespace ExpenseTrackerWasmWebApp.Services;

public class CachedDataService(IHttpClientFactory httpClientFactory)
{
    public bool IsInitialized{get; set;} = false;
    private List<AccountDto> _accounts = [];
    private List<CategoryDto> _categories = [];
    private List<TagDto> _tags = [];
    private UserPreferenceDto _userPreferenceDto = new UserPreferenceDto(){DarkMode = false};
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        await RefreshAll();
        IsInitialized = true;
    }
    public async Task RefreshAll()
    {
        try
        {
            await RefreshAccountsAsync();
            await RefreshCategoriesAsync();
            await RefreshTagsAsync();
            await RefreshUserPreferencesAsync();
        }
        catch
        {
        }
    }


    public List<AccountDto> GetAccounts()
    {
        return _accounts;
    }

    public List<CategoryDto> GetCategories()
    {
        return _categories;
    }
    public List<TagDto> GetTags()
    {
        return _tags;
    }
    public UserPreferenceDto GetUserPreferences()
    {
        return _userPreferenceDto;
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
    public async Task RefreshUserPreferencesAsync()
    {
        try
        {
            var http = _httpClientFactory.CreateClient("WebAPI");
            _userPreferenceDto = await http.GetFromJsonAsync<UserPreferenceDto>("api/userpreferences");
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
        _accounts = [];
        _categories = [];
        _tags = [];
        _userPreferenceDto = new UserPreferenceDto(){DarkMode = false};
    }
}
