using MimeMapping;

namespace KuCloud.Core.Domains.StorageAggregate;

// TODO - 是否需要将 Folder 和 FileNode 分开，不该继承同一个基类
public abstract class StorageNode : BasicEntity<long>, IAggregateRoot
{
    // EF Core required
    protected StorageNode() { }

    protected StorageNode(StorageType type, string name, Folder? parent)
    {
        Type = type;
        Name = name;

        SetParent(parent);
    }

    public StorageType Type { get; set; } = null!;

    private string _name = null!;

    public string Name
    {
        get => _name;
        set => _name = Guard.Against.CheckInvalidPath(value);
    }

    public string ContentType => MimeUtility.GetMimeMapping(Name);

    public Folder? Parent { get; private set; }

    public long? ParentId { get; set; }

    public bool IsRoot => Parent == null;

    public void SetParent(Folder? parent)
    {
        if (parent == Parent)
        {
            return;
        }

        if (parent is not null && this is Folder folder)
        {
            if (parent == folder)
            {
                // TODO - 捕捉异常，然后返回为正常的信息
                throw new InvalidOperationException("Cannot set parent to self");
            }

            if (folder.IsAncestorOf(parent))
            {
                throw new InvalidOperationException("Cannot set parent to a descendant");
            }
        }

        // remove from old parent
        Parent?.RemoveChild(this);

        // set new parent
        ParentId = parent?.Id;
        Parent = parent;
        Parent?.AddChild(this);
    }
}
