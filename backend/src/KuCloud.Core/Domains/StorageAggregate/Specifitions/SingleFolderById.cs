using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class SingleFolderById : Specification<Folder>, ISingleResultSpecification<Folder>
{
    public SingleFolderById(long id, bool readOnly = false, bool includeChildren = true, bool includeDescendant = false,
        bool includeDeleted = false)
    {
        // TODO - 查询时如何判断是否已经删除（父节点已经删除，但是子节点还在）

        Query.Where(x => x.Id == id);

        Query.Include(e => e.AncestorRelations).ThenInclude(e => e.Ancestor);

        if (includeDescendant)
        {
            Query.Include(e => e.DescendantRelations).ThenInclude(e => e.Descendant);
        }

        if (includeChildren)
        {
            Query.Include(e => e.Children);
        }

        if (includeDeleted)
        {
            Query.IgnoreQueryFilters();
        }

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}

/// <summary>
///     查询文件夹的所有信息，包括祖先、后代、子节点
///     FIXME: 性能问题 查出祖先链，并将祖先的所有后代关系也查出来
/// </summary>
public sealed class SingleFolderForAllInfo : Specification<Folder>, ISingleResultSpecification<Folder>
{
    public SingleFolderForAllInfo(long id, bool readOnly = false)
    {
        Query.Where(x => x.Id == id);

        Query.Include(e => e.AncestorRelations).ThenInclude(e => e.Ancestor).ThenInclude(e => e.DescendantRelations);
        Query.Include(e => e.DescendantRelations).ThenInclude(e => e.Descendant);
        Query.Include(e => e.Children);

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}
