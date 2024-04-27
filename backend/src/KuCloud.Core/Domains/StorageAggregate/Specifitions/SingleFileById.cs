using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public sealed class SingleFileById : Specification<FileNode>, ISingleResultSpecification<FileNode>
{
    public SingleFileById(
        long id,
        bool readOnly = false,
        bool includeParent = true,
        bool includeDeleted = false
    )
    {
        Query.Where(e => e.Id == id);

        if (includeParent)
        {
            Query.Include(e => e.Parent).ThenInclude(e => e!.Children);
        }

        if (includeDeleted)
        {
            Query.IgnoreQueryFilters();
        }
        else
        {
            Query.Where(e => e.Parent != null || !e.Parent!.AuditInfo.IsDelete);
        }

        if (readOnly)
        {
            Query.AsNoTracking();
        }
    }
}
