namespace KuCloud.SharedKernel;

public record AuditRecord(DateTime LastUpdateTime, DateTime? DeletionTime, bool IsDelete);

public interface IAuditable
{
    public AuditInfo AuditInfo { get; set; }
}


public abstract class BasicEntity<TId> : EntityBase<TId>, IAuditable where TId : struct, IEquatable<TId>
{
    public AuditInfo AuditInfo { get; set; } = new();
}

public abstract record BasicDto
{
    public AuditRecord AuditRecord { get; set; } = null!;
}
