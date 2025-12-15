
using ApexCharts;
using ExpenseTrackerWasmWebApp.Features.Accounts.Services;
using ExpenseTrackerWasmWebApp.Features.Budgets.Services;
using ExpenseTrackerWasmWebApp.Features.Categories.Services;
using ExpenseTrackerWasmWebApp.Features.Dashboard.Services;
using ExpenseTrackerWasmWebApp.Features.DataSeeding.Services;
using ExpenseTrackerWasmWebApp.Features.Tags.Services;
using ExpenseTrackerWasmWebApp.Features.Transactions.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

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
    services.AddTransient<TransactionService>();
}

