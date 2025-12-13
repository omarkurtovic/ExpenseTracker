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

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// Authentication and Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();


builder.Services.AddHttpClient("WebAPI", 
        client => client.BaseAddress = new Uri("http://localhost:5001/"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"));


AddServices(builder.Services);
await builder.Build().RunAsync();

void AddServices(IServiceCollection services)
{
    // ApexCharts Configuration
    ConfigureApexCharts(services);

    // Custom Services
    ConfigureCustomServices(services);

    // MudBlazor
    ConfigureMudBlazor(services);
}

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
    services.AddSingleton<CachedDataService>();
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

