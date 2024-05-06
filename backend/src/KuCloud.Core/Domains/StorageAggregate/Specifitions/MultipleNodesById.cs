using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class MultipleNodesById : Specification<StorageNode>
{
    public MultipleNodesById(
        IEnumerable<long> ids,
        bool includeDeleted = false,
        bool includeFolderNesting = false,
        bool readOnly = false
    )
    {
        Query.Include(e => e.Parent);

        Query.Where(e => ids.Contains(e.Id));

        if (includeFolderNesting)
        {
            Query.Include(e => (e as Folder)!.Children);

            Query.Include(e => (e as Folder)!.AncestorRelations)
                .ThenInclude(e => e.Ancestor)
                .ThenInclude(e => e.DescendantRelations);

            Query.Include(e => (e as Folder)!.DescendantRelations)
                .ThenInclude(e => e.Descendant);
        }

        if (includeDeleted)
        {
            Query.IgnoreQueryFilters();
        }
        else
        {
            Query.Where(e =>
                !e.AuditInfo.IsDelete
             && (e.Parent == null || !e.Parent.AuditInfo.IsDelete)
             && (e.Type != StorageType.Folder
              || (e as Folder)!.AncestorRelations.All(r => !r.Ancestor.AuditInfo.IsDelete)
                )
            );
        }

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}
