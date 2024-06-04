using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KuCloud.Infrastructure.Data;

internal sealed class SetAuditInfoInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        SetAuditInfo(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(eventData.Context);
        SetAuditInfo(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetAuditInfo(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            var entity = entry.Entity;
            if (entity is not IAuditable auditable) continue;

            var now = DateTime.UtcNow;
            switch (entry.State)
            {
                case EntityState.Added:
                    auditable.AuditInfo.SetCreateInfo(now);
                    break;
                case EntityState.Modified:
                    var hasModified = entry.Properties
                        .Where(p => p.IsModified)
                        .Where(p => p.Metadata.Name != nameof(IAuditable.AuditInfo))
                        .Any(
                            propertyEntry => propertyEntry.OriginalValue != propertyEntry.CurrentValue
                        );
                    if (!hasModified) break;
                    auditable.AuditInfo.SetModifyInfo(now);
                    break;
                case EntityState.Deleted:
                    if (!auditable.AuditInfo.IsDelete)
                    {
                        auditable.AuditInfo.SetDeleteInfo(now);
                        entry.State = EntityState.Modified;
                    }
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
