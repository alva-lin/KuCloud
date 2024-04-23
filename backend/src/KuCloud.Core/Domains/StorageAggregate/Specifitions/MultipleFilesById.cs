using Ardalis.Specification;

namespace KuCloud.Core.Domains.StorageAggregate;

public class MultipleFilesById : Specification<FileNode>
{
    public MultipleFilesById(
        long[] ids,
        bool readOnly = false,
        bool includeParent = true,
        bool includeDeleted = false
        )
    {
        Query.Where(e => ids.Contains(e.Id));

        if (includeParent)
        {
            Query.Include(e => e.Parent);
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
