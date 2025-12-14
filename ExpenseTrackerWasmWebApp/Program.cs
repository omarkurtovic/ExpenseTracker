using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ExpenseTrackerWasmWebApp;
using ExpenseTrackerWasmWebApp.Services;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ApexCharts;
using MudBlazor;
using ExpenseTrackerWasmWebApp.Features.Transactions.Services;
using ExpenseTrackerWasmWebApp.Features.Accounts.Services;
using ExpenseTrackerWasmWebApp.Features.Categories.Services;
using ExpenseTrackerWasmWebApp.Features.Budgets.Services;
using ExpenseTrackerWasmWebApp.Features.Dashboard.Services;
using ExpenseTrackerWasmWebApp.Features.Tags.Services;
using ExpenseTrackerWasmWebApp.Features.DataSeeding.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Authentication and Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddCascadingAuthenticationState();

// so that each request contains the auth token
builder.Services.AddHttpClient("WebAPI", 
        client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// so we can inject IHttpClient directly
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"));


ConfigureApexCharts(builder.Services);
ConfigureMudBlazor(builder.Services);
ConfigureCustomServices(builder.Services);
await builder.Build().RunAsync();


void ConfigureApexCharts(IServiceCollection services)
{
    services.AddApexCharts(e =>
    {
        e.GlobalOptions = new ApexChartBaseOptions
        {
            Debug = false,
            Theme = new Theme { Mode = Mode.Light, Palette = PaletteType.Palette6 },
            Chart = new Chart { Background = "transparent" }
        };
    });
}

void ConfigureMudBlazor(IServiceCollection services)
{
    services.AddMudServices();
    services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = false;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 8000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    });
}


void ConfigureCustomServices(IServiceCollection services)
{
    services.AddTransient<AccountService>();
    services.AddTransient<CategoryService>();
    services.AddTransient<BudgetService>();
    services.AddTransient<DashboardService>();
    services.AddTransient<TagService>();
    services.AddTransient<DataSeedService>();
    services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
    services.AddScoped<BaseAddressAuthorizationMessageHandler>();
    services.AddTransient<TransactionService>();
}


public class BaseAddressAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public BaseAddressAuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

