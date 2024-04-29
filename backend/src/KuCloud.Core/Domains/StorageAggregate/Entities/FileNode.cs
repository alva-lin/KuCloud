using MimeMapping;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class FileNode : StorageNode
{
    // EF Core required
    private FileNode() { }

    public FileNode(Folder? parent, string name, string path, long size) : base(StorageType.File, name, parent)
    {
        Path = path;
        Size = size;
    }

    public string ContentType => MimeUtility.GetMimeMapping(Name);

    public string Path { get; set; } = null!;

    public long Size { get; set; }
}
