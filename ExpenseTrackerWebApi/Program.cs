using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerWebApi.Database;
using ExpenseTrackerWebApi.Features.SharedKernel.Behaviors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
ConfigureDatabase(builder.Services, builder.Environment);
ConfigureAuthentication(builder.Services);
ConfigureMediatR(builder.Services);

var app = builder.Build();

InitializeDatabase(app);

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
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