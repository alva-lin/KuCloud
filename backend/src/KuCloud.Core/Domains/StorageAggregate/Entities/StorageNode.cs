using Ardalis.GuardClauses;
using Ardalis.SharedKernel;

namespace KuCloud.Core.Domains.StorageAggregate;

public abstract class StorageNode : BasicEntity<long>, IAggregateRoot
{
    // EF Core required
    protected StorageNode() { }

    protected StorageNode(StorageType type, string name, Folder? parent)
    {
        Type = type;

        Name = Guard.Against.CheckInvalidPath(name);

        Parent = parent;
        ParentId = parent?.Id;
        parent?.AddChild(this);
    }

    public StorageType Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public Folder? Parent { get; set; }

    public long? ParentId { get; set; }

    public string Path => Parent?.Path + "/" + Name;

    public bool IsRoot => Parent == null;
}
