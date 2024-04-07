using KuCloud.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace KuCloud.FunctionalTests;

public class AppFixture(IMessageSink sink) : AppFixture<Program>(sink)
{
    protected override async Task SetupAsync()
    {
        // place one-time setup code here
        Fake.Locale = "en";

        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Reset Sqlite database for each test run
        // If using a real database, you'll likely want to remove this step.
        await db.Database.EnsureDeletedAsync();

        // Ensure the database is created and all migrations are run.
        await db.Database.EnsureCreatedAsync();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppFixture>>();

        try
        {
            // Can also skip creating the items
            // if (!db.ProductTypes.Any())
            {
                // Seed the database with test data.
                SeedData.PopulateTestData(db);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred seeding the database with test messages. Error: {ExceptionMessage}",
                e.Message);
        }
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        // do host builder config here
        base.ConfigureApp(a);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // do test service registration here
        //// Remove the app's ApplicationDbContext registration.
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        //// This should be set for each individual test run
        var inMemoryCollectionName = Guid.NewGuid().ToString();

        //// Add ApplicationDbContext using an in-memory database for testing.
        services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase(inMemoryCollectionName); });
    }

    protected override Task TearDownAsync() =>
        // do cleanups here
        Task.CompletedTask;
}
