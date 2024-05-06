namespace KuCloud.Core.Domains.StorageAggregate;

/// <summary>
///     The relation of folder nesting
/// </summary>
public sealed class FolderNesting
{
    public long AncestorId { get; private set; }

    private readonly Folder _ancestor = null!;

    public Folder Ancestor
    {
        get => _ancestor;
        init
        {
            _ancestor = value;
            AncestorId = value.Id;
        }
    }

    private readonly Folder _descendant = null!;

    public long DescendantId { get; private set; }


    public Folder Descendant
    {
        get => _descendant;
        init
        {
            _descendant = value;
            DescendantId = value.Id;
        }
    }

    public int Depth { get; set; }
}
