namespace KuCloud.Core;

public class AuditInfo : ValueObject
{
    public bool IsDelete { get; private set; }

    public string Creator { get; private set; } = null!;

    public string CreatorId { get; private set; } = null!;

    public DateTime CreationTime { get; private set; }

    public DateTime? ModifiedTime { get; private set; }

    public DateTime? DeletionTime { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CreatorId;
        yield return Creator;
        yield return CreationTime;
        yield return ModifiedTime ?? DateTime.MinValue;
        yield return IsDelete;
    }

    public void SetCreateInfo(string creator, string creatorId, DateTime? creationTime = null)
    {
        Creator = creator;
        CreatorId = creatorId;
        SetCreateInfo(creationTime);
    }

    public void SetCreateInfo(DateTime? creationTime = null)
    {
        Creator = "System";
        CreatorId = "System";
        CreationTime = creationTime ?? DateTime.UtcNow;
        ModifiedTime = null;
        IsDelete = false;
    }

    public void SetModifyInfo(DateTime? modifiedTime = null)
    {
        ModifiedTime = modifiedTime ?? DateTime.UtcNow;
    }

    public void SetDeleteInfo(DateTime? deletionTime = null)
    {
        DeletionTime = deletionTime ?? DateTime.UtcNow;
        IsDelete = true;
    }

    public void Restore()
    {
        DeletionTime = null;
        IsDelete = false;
    }

    public AuditRecord ToRecord() => new(CreationTime, ModifiedTime, DeletionTime, IsDelete);
}

public record AuditRecord(DateTime CreationTime, DateTime? ModifiedTime, DateTime? DeletionTime, bool IsDelete);

public interface IAuditable
{
    public AuditInfo AuditInfo { get; set; }
}

public abstract class BasicEntity<TId> : EntityBase<TId>, IAuditable
    where TId : struct, IEquatable<TId>
{
    public AuditInfo AuditInfo { get; set; } = new();
}

public abstract record BasicDto
{
    public AuditRecord AuditRecord { get; set; } = null!;
}
