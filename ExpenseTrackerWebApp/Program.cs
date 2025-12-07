using System.Globalization;
using ApexCharts;
using ExpenseTrackerWebApp.Components;
using ExpenseTrackerWebApp.Database;
using ExpenseTrackerWebApp.Services;
using ExpenseTrackerWebApp.Features.SharedKernel.Behaviors;
using ExpenseTrackerWebApp.Features.SharedKernel.Behaviours;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

var builder = WebApplication.CreateBuilder(args);

SetupCulture();
ConfigureServices(builder.Services, builder.Environment);


var app = builder.Build();

InitalizeDatabase(app);
ConfigurePipeline(app);

app.Run();


void SetupCulture()
{
    var culture = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}

void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
{
    services.AddControllers();
    services.AddRazorComponents().AddInteractiveServerComponents();
    
    ConfigureDatabase(services, env);
    ConfigureAuthentication(services);
    ConfigureMediatR(services);
    ConfigureValidation(services);
    ConfigureMudBlazor(services);
    ConfigureApexCharts(services);
    ConfigureCustomServices(services);
    
    services.AddHttpContextAccessor();
    services.AddLogging();
}

void ConfigureDatabase(IServiceCollection services, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=app.db"));
    else
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=/home/app.db"));
}

void ConfigureAuthentication(IServiceCollection services)
{
    services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
    });

    services.AddAuthentication();
    services.AddAuthorization();
    services.AddCascadingAuthenticationState();
}

void ConfigureMediatR(IServiceCollection services)
{
    services.AddMediatR(options =>
    {
        options.RegisterServicesFromAssembly(typeof(Program).Assembly);
        options.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        options.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });
}

void ConfigureValidation(IServiceCollection services)
{
    services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
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

void ConfigureCustomServices(IServiceCollection services)
{
    services.AddScoped<ICurrentUserService, CurrentUserService>();
    services.AddScoped<TagService>();
    
}

void InitalizeDatabase(WebApplication app)
{

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var categoriesNeedingDefaults = db.Categories
        .Where(c => string.IsNullOrEmpty(c.Color) || string.IsNullOrEmpty(c.Icon))
        .ToList();

    if (categoriesNeedingDefaults.Count != 0)
    {
        foreach (var category in categoriesNeedingDefaults)
        {
            category.Color ??= "#A9A9A9";
            category.Icon ??= "Icons.Material.Filled.Category";
        }
        db.SaveChanges();
    }

    var accountsNeedingDefaults = db.Accounts
        .Where(a => string.IsNullOrEmpty(a.Color) || string.IsNullOrEmpty(a.Icon))
        .ToList();

    if (accountsNeedingDefaults.Count != 0)
    {
        var iconColors = new[] { "#FF5733", "#33C1FF", "#33FF57", "#FF33A8" };
        var icons = new[]
        {
            "Icons.Material.Filled.Payments",
            "Icons.Material.Filled.AccountBalance",
            "Icons.Material.Filled.Wallet",
            "Icons.Material.Filled.CreditCard"
        };

        for (int i = 0; i < accountsNeedingDefaults.Count; i++)
        {
            var account = accountsNeedingDefaults[i];
            account.Color ??= iconColors[i % iconColors.Length];
            account.Icon ??= icons[i % icons.Length];
        }
        db.SaveChanges();
    }
}

void ConfigurePipeline(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAntiforgery();

    app.MapStaticAssets();


    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/login" && context.User.Identity?.IsAuthenticated == true)
        {
            context.Response.Redirect("/");
            return;
        }
        await next();
    });

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

}

