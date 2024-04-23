namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class FileNode : StorageNode
{
    // EF Core required
    private FileNode() { }

    public FileNode(Folder? parent, string name, string contentType, long size) : base(StorageType.File, name, parent)
    {
        ContentType = Guard.Against.NullOrWhiteSpace(contentType);
        Size = size;
    }

    public string ContentType { get; set; } = null!;

    public long Size { get; set; }
}
