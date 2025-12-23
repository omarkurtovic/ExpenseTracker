using ApexCharts;
using ExpenseTrackerWasmWebApp;
using ExpenseTrackerWasmWebApp.Features.Accounts.Services;
using ExpenseTrackerWasmWebApp.Features.Budgets.Services;
using ExpenseTrackerWasmWebApp.Features.Categories.Services;
using ExpenseTrackerWasmWebApp.Features.Dashboard.Services;
using ExpenseTrackerWasmWebApp.Features.DataSeeding.Services;
using ExpenseTrackerWasmWebApp.Features.Tags.Services;
using ExpenseTrackerWasmWebApp.Features.Transactions.Services;
using ExpenseTrackerWebApi;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Auth;
using ExpenseTrackerWebApi.Features.SharedKernel.Behaviors;
using ExpenseTrackerWebApi.Features.SharedKernel.Components;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMudServices();
builder.Services.AddScoped<IdentityRedirectManager>();

// potrebni su nam ovi ovdje zbog preloading
builder.Services.AddHttpClient();

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BudgetService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<DataSeedService>();
builder.Services.AddScoped<TransactionService>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
builder.Services.AddAuthorization();

ConfigureDatabase(builder.Services, builder.Environment);

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
ConfigureMediatR(builder.Services);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
});

var app = builder.Build();

// 1. Exception handling

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// 2. HTTPS redirection
app.UseHttpsRedirection();

// 3. Static files
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapStaticAssets();

// 4. Routing
app.UseRouting();

// 5. Authentication
app.UseAuthentication();

// 6. Authorization
app.UseAuthorization();

// 7. Antiforgery
app.UseAntiforgery();

// 8. ENDPOINTS
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ExpenseTrackerWasmWebApp._Imports).Assembly);

// 9. Database initialization
InitializeDatabase(app);

// 10. FALLBACK
app.MapFallbackToFile("index.html");

app.Run();



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
        
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "ExpenseTrackerWebApi",
            ValidAudience = "ExpenseTrackerWebApiUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLongForSecurity"))
        };
    });
    services.AddAuthorization();

    
}

void ConfigureMediatR(IServiceCollection services)
{
    services.AddMediatR(options =>
    {
        options.RegisterServicesFromAssembly(typeof(Program).Assembly);
        options.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        options.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });
    services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
}

void InitializeDatabase(WebApplication app)
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