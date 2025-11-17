using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Database;
using ExpenseTracker.Database.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Tests;

public class SqliteInMemoryAccountControllerTest : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public SqliteInMemoryAccountControllerTest()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new AppDbContext(_contextOptions);

        if (context.Database.EnsureCreated())
        {
            var user1 = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(), // Identity uses string IDs by default
            UserName = "testuser1@test.com",
            NormalizedUserName = "TESTUSER1@TEST.COM",
            Email = "testuser1@test.com",
            NormalizedEmail = "TESTUSER1@TEST.COM",
            EmailConfirmed = true
        };
        
        var user2 = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser2@test.com",
            NormalizedUserName = "TESTUSER2@TEST.COM",
            Email = "testuser2@test.com",
            NormalizedEmail = "TESTUSER2@TEST.COM",
            EmailConfirmed = true
        };
        
        context.Users.AddRange(user1, user2);
        context.SaveChanges();
        
        context.Accounts.AddRange(
            new Account { Name = "Account1", InitialBalance=1000.0m, IdentityUserId = user1.Id },
            new Account { Name = "Account2", InitialBalance = 1500.0m, IdentityUserId = user2.Id }
        );
        context.SaveChanges();
        }
    }

    AppDbContext CreateContext() => new AppDbContext(_contextOptions);

    public void Dispose() => _connection.Dispose();
    
    [Fact]
    public async Task GetAsync()
    {
        using var context = CreateContext();
        var controller = new AccountService(context, null);

        var account = await controller.GetAsync(1);

        Assert.Equal("Account1", account.Name);
        Assert.Equal(1000.0m, account.InitialBalance);
    }
}