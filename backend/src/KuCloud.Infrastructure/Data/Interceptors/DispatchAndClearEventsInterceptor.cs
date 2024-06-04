using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KuCloud.Infrastructure.Data;

public sealed class DispatchAndClearEventsInterceptor(IDomainEventDispatcher? dispatcher) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        DispatchAndClearEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        await DispatchAndClearEvents(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchAndClearEvents(DbContext dbContext)
    {
        // ignore events if no dispatcher provided
        if (dispatcher == null) return;

        // dispatch events only if save was successful
        var entitiesWithEvents = dbContext.ChangeTracker.Entries<EntityBase>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();

        await dispatcher.DispatchAndClearEvents(entitiesWithEvents);
    }
}
