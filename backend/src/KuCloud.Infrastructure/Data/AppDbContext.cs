using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartEnum.EFCore;

namespace KuCloud.Infrastructure.Data;

public sealed partial class AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyBasicEntityConfigurationsFromAssembly(
            logger,
            [
                typeof(AppDbContext).Assembly,
                typeof(BasicEntity<>).Assembly,
            ]
        );

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.ConfigureSmartEnum();
    }
}
