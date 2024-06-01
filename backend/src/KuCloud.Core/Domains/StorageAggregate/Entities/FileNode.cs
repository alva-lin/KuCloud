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

    // TODO - 是否需要一个额外的 PathMap，来映射 FileNodeId -> Path
    public string Path { get; set; } = null!;

    public long Size { get; set; }

    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Name);

    public string Extension => System.IO.Path.GetExtension(Name);
}
