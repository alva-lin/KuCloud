namespace KuCloud.Core.Domains.StorageAggregate;

public record FileNodeDto : StorageNodeDto
{
    public Dictionary<string, object> Attributes { get; } = new();

    public FileNodeDto(FileNode file) : base(file)
    {
        Attributes.Add("Size", file.Size);
    }
}
