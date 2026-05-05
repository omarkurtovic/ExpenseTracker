using ExpenseTrackerSharedCL.Features.Accounts.Services;
using ExpenseTrackerSharedCL.Features.Budgets.Services;
using ExpenseTrackerSharedCL.Features.Categories;
using ExpenseTrackerSharedCL.Features.Dashboard;
using ExpenseTrackerSharedCL.Features.Logging.Services;
using ExpenseTrackerSharedCL.Features.Tags.Service;
using ExpenseTrackerSharedCL.Features.Transactions.Services;
using ExpenseTrackerSharedCL.Features.UserPreferences.Services;
using ExpenseTrackerWasmWebApp.Features.Logging.Services;
using ExpenseTrackerWebApi;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.Accounts.Services;
using ExpenseTrackerWebApi.Features.Auth;
using ExpenseTrackerWebApi.Features.Budgets.Services;
using ExpenseTrackerWebApi.Features.Categories.Services;
using ExpenseTrackerWebApi.Features.Dashboard;
using ExpenseTrackerWebApi.Features.Logging.Services;
using ExpenseTrackerWebApi.Features.SharedKernel.Behaviors;
using ExpenseTrackerWebApi.Features.Tags.Services;
using ExpenseTrackerWebApi.Features.Transactions.Services;
using ExpenseTrackerWebApi.Features.UserPreferences.Services;
using ExpenseTrackerWebApi.Middleware;
using ExpenseTrackerWebApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// AddAuthenticationStateSerialization is neccessary if we want to have access to
// user claims on the client - it seralizes the claims and makes them avaliable like username
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents()
    .AddAuthenticationStateSerialization();


builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMudServices();
builder.Services.AddScoped<IdentityRedirectManager>();

// need this here for preloading to work
builder.Services.AddScoped<IDashboardService, DashboardServiceServer>();
builder.Services.AddScoped<IAccountService, AccountServiceServer>();
builder.Services.AddScoped<ICategoryService, CategoryServiceServer>();
builder.Services.AddScoped<IBudgetService, BudgetServiceServer>();
builder.Services.AddScoped<ITransactionService, TransactionServiceServer>();
builder.Services.AddScoped<ITagService, TagServiceServer>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceServiceServer>();
builder.Services.AddScoped<ILogService, LogServiceServer>();

builder.Services.AddTransient<DatabaseSeeder>();

// this is needed for code outside of controllers
// so we can access the current logged in user
builder.Services.AddHttpContextAccessor();


// cookie based authenfication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
builder.Services.AddAuthorization();

ConfigureDatabase(builder.Services, builder.Environment);

builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
ConfigureMediatR(builder.Services);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(ExpenseTrackerWasmWebApp._Imports).Assembly);

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

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
