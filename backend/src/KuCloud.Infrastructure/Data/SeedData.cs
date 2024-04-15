using KuCloud.Core.ContributorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KuCloud.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var dbContext = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>(),
            serviceProvider.GetRequiredService<ILogger<AppDbContext>>(),
            null
        );
        if (dbContext.Folders.Any()) return; // DB has been seeded

        PopulateTestData(dbContext);
    }

    public static void PopulateTestData(AppDbContext dbContext)
    {
        foreach (var folder in dbContext.Folders) dbContext.Remove(folder);
        dbContext.SaveChanges();

        dbContext.SaveChanges();
    }
}
