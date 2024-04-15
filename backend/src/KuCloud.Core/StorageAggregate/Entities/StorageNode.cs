using Ardalis.GuardClauses;
using Ardalis.SharedKernel;

namespace KuCloud.Core.StorageAggregate;

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

public sealed class Folder : StorageNode
{
    // EF Core required
    private Folder() { }

    public Folder(string name, Folder? parent) : base(StorageType.Folder, name, parent) { }

    private readonly List<StorageNode> _children = [];

    public IReadOnlyList<StorageNode> Children => _children.AsReadOnly();

    public void AddChild(StorageNode node)
    {
        _children.Add(node);
    }
}

public sealed class FileNode : StorageNode
{
    // EF Core required
    private FileNode() { }

    public FileNode(string name, Folder? parent, string contentType, long size) : base(StorageType.File, name, parent)
    {
        ContentType = Guard.Against.NullOrWhiteSpace(contentType);
        Size = size;
    }

    public string ContentType { get; set; } = null!;

    public long Size { get; set; }
}
