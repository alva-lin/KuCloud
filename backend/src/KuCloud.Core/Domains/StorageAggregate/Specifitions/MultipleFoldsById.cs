using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class MultipleFoldsById : Specification<Folder>
{
    public MultipleFoldsById(
        long[] ids,
        bool readOnly = false,
        bool includeChildren = true,
        bool includeDescendant = false,
        bool includeDeleted = false
    )
    {
        Query.Where(e => ids.Contains(e.Id));

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
