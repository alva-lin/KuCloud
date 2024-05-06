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
            serviceProvider.GetRequiredService<ILogger<AppDbContext>>(),
            null
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

        var folder2 = new Folder("Folder2", folder);
        dbContext.Folders.Add(folder2);

        var file1 = new FileNode(folder1, "file1.txt", "text/plain", 0L);
        dbContext.FileNodes.Add(file1);

        var file2 = new FileNode(folder1, "file2.txt", "text/plain", 0L);
        dbContext.FileNodes.Add(file2);

        var file3 = new FileNode(folder2, "file3.txt", "text/plain", 0L);
        dbContext.FileNodes.Add(file3);

        dbContext.SaveChanges();
    }
}
