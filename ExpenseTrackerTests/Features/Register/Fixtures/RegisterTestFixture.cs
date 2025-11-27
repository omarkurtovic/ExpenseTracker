using System.Data.Common;
using ExpenseTrackerWebApp.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerTests.Features.Register.Fixtures
{
    public class RegisterTestFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public RegisterTestFixture()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        public AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
    }
}
