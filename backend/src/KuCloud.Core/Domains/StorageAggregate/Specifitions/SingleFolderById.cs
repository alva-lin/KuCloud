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
