namespace KuCloud.Core.Domains.StorageAggregate;

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