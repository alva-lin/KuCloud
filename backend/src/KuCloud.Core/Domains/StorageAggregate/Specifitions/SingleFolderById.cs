using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class SingleFolderById : Specification<Folder>, ISingleResultSpecification<Folder>
{
    public SingleFolderById(
        long id,
        bool readOnly = false,
        bool includeChildren = true,
        bool includeDescendant = false,
        bool includeDeleted = false
    )
    {

        Query.Where(x => x.Id == id);

        Query.Include(e => e.AncestorRelations).ThenInclude(e => e.Ancestor);

        if (includeDescendant)
        {
            Query.Include(e => e.DescendantRelations).ThenInclude(e => e.Descendant);
        }

        if (includeChildren)
        {
            Query.Include(e => e.Children.OrderBy(e => e.Type).ThenBy(e => e.Name));
        }

        if (includeDeleted)
        {
            Query.IgnoreQueryFilters();
        }
        else
        {
            Query.Where(e => e.AncestorRelations.All(r => !r.Ancestor.AuditInfo.IsDelete));
        }

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}
