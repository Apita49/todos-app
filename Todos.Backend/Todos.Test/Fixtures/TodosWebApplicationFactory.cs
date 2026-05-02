using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todos.Infrastructure;

namespace Todos.Test.Fixtures
{
    public class TodosWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Clear existing configuration and add test configuration
                config.Sources.Clear();
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "AllowedOrigins:0", "http://localhost" },
                    { "AllowedOrigins:1", "http://localhost:3000" },
                    { "ConnectionStrings:DefaultConnection", "Data Source=file::memory:?cache=shared" }
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove the real database context
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Use in-memory SQLite database for testing (shared across connections)
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite("Data Source=file::memory:?cache=shared");
                });
            });
        }

        public async Task InitializeAsync()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
            }
        }

        public async Task ResetDatabaseAsync()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // Delete all todos instead of dropping the database
                dbContext.Todos.RemoveRange(dbContext.Todos);
                await dbContext.SaveChangesAsync();
            }
        }

        public AppDbContext GetDbContext()
        {
            var scope = Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }
    }
}
