using System.Reflection;
using Ardalis.SharedKernel;
using KuCloud.Core;
using KuCloud.Core.ContributorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using SmartEnum.EFCore;

namespace KuCloud.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    private readonly ILogger<AppDbContext> _logger;
    private readonly IDomainEventDispatcher? _dispatcher;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ILogger<AppDbContext> logger,
        IDomainEventDispatcher? dispatcher
    ) : base(options)
    {
        _logger = logger;
        _dispatcher = dispatcher;

        ChangeTracker.StateChanged += UpdateAuditInfo;
        ChangeTracker.Tracked += UpdateAuditInfo;
    }

    public DbSet<Contributor> Contributors => Set<Contributor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyBasicEntityConfigurationsFromAssembly(
            _logger,
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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (_dispatcher == null) return result;

        // dispatch events only if save was successful
        var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

        return result;
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    private static void UpdateAuditInfo(object? sender, EntityEntryEventArgs e)
    {
        if (e.Entry.Entity is not IAuditable entity) return;

        var now = DateTime.UtcNow;
        switch (e.Entry.State)
        {
            case EntityState.Deleted:
                entity.AuditInfo.SetDeleteInfo(now);
                e.Entry.State = EntityState.Modified;
                break;
            case EntityState.Modified:
                entity.AuditInfo.SetModifyInfo(now);
                break;
            case EntityState.Added:
                entity.AuditInfo.SetCreateInfo(now);
                break;
            case EntityState.Detached:
                break;
            case EntityState.Unchanged:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
