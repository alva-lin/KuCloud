namespace KuCloud.Core.Domains.StorageAggregate;

public record FolderDto : StorageNodeDto
{
    public List<(FolderDto Ancestor, int Depth)>? Ancestors { get; set; }

    public List<StorageNodeDto>? Children { get; set; }

    public FolderDto(Folder folder, bool ignoreChildren = false) : base(folder)
    {
        if (!ignoreChildren)
        {
            Children = folder.Children.Select<StorageNode, StorageNodeDto>(e => e switch
            {
                FileNode file => new FileNodeDto(file),
                Folder subFolder => new FolderDto(subFolder, true),
                _ => throw new NotSupportedException()
            }).ToList();
        }

        Ancestors = folder.Ancestors.Select(e => (new FolderDto(e.Ancestor, true), e.Depth)).ToList();
    }
}
