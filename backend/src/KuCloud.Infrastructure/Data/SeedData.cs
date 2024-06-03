using KuCloud.Core.Domains.StorageAggregate;
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
            serviceProvider.GetRequiredService<ILogger<AppDbContext>>()
        );
        if (dbContext.Folders.Any()) return; // DB has been seeded

        PopulateTestData(dbContext);
    }

    public static void PopulateTestData(AppDbContext dbContext)
    {
        AddStorageTestData(dbContext);

        dbContext.SaveChanges();
    }

    private static void AddStorageTestData(AppDbContext dbContext)
    {
        dbContext.FileNodes.ExecuteDelete();
        dbContext.Folders.ExecuteDelete();

        var folder = new Folder("Root", null);
        dbContext.Folders.Add(folder);

        var folder1 = new Folder("Folder1", folder);
        dbContext.Folders.Add(folder1);

        var folder11 = new Folder("Folder1-1", folder1);
        dbContext.Folders.Add(folder11);

        var folder2 = new Folder("Folder2", folder);
        dbContext.Folders.Add(folder2);

        dbContext.SaveChanges();
    }
}
